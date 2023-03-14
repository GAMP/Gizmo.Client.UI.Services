using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserAgreementsService : ViewStateServiceBase<UserAgreementsViewState>
    {
        #region CONSTRUCTOR
        public UserAgreementsService(UserAgreementsViewState viewState,
            ILogger<UserAgreementsService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadUserAgreementsAsync(int? userId = null, CancellationToken cToken = default)
        {
            ViewState.UserId = userId;

            var userAgreements = await _gizmoClient.UserAgreementsGetAsync(new UserAgreementsFilter(), cToken);
            var tmpUserAgreements = userAgreements.Data.Select(a => new UserAgreementViewState()
            {
                Id = a.Id,
                Name = a.Name,
                Agreement = a.Agreement,
                IsRejectable = a.IsRejectable,
                IgnoreState = a.IgnoreState,
                AcceptState = UserAgreementAcceptState.None
            }).ToList();

            if (ViewState.UserId.HasValue)
            {
                var userAgreementStates = await _gizmoClient.UserAgreementsStatesGetAsync(cToken);
                foreach (var item in userAgreementStates)
                {
                    var userAgreement = tmpUserAgreements.Where(a => a.Id == item.UserAgreementId).FirstOrDefault();

                    if (userAgreement != null)
                    {
                        userAgreement.AcceptState = item.AcceptState;
                    }
                }
            }

            ViewState.UserAgreements = tmpUserAgreements.Where(a => a.AcceptState != UserAgreementAcceptState.Accepted || a.IgnoreState).ToList();

            ViewState.CurrentUserAgreementIndex = 0;
            ViewState.CurrentUserAgreement = ViewState.UserAgreements[ViewState.CurrentUserAgreementIndex.Value];

            ViewState.RaiseChanged();
        }

        public Task AcceptCurrentUserAgreementAsync()
        {
            if (ViewState.CurrentUserAgreement != null)
            {
                ViewState.CurrentUserAgreement.AcceptState = UserAgreementAcceptState.Accepted;

                if (ViewState.CurrentUserAgreementIndex != ViewState.UserAgreements.Count() - 1)
                {
                    ViewState.CurrentUserAgreementIndex += 1;
                    ViewState.CurrentUserAgreement = ViewState.UserAgreements[ViewState.CurrentUserAgreementIndex.Value];
                }
                else
                {
                    ViewState.CurrentUserAgreementIndex = null;
                    ViewState.CurrentUserAgreement = null;
                }

                ViewState.RaiseChanged();
            }

            return Task.CompletedTask;
        }

        public Task RejectCurrentUserAgreementAsync()
        {
            if (ViewState.CurrentUserAgreement != null)
            {
                ViewState.CurrentUserAgreement.AcceptState = UserAgreementAcceptState.Rejected;

                if (ViewState.CurrentUserAgreementIndex != ViewState.UserAgreements.Count() - 1)
                {
                    ViewState.CurrentUserAgreementIndex += 1;
                    ViewState.CurrentUserAgreement = ViewState.UserAgreements[ViewState.CurrentUserAgreementIndex.Value];
                }
                else
                {
                    ViewState.CurrentUserAgreementIndex = null;
                    ViewState.CurrentUserAgreement = null;
                }

                ViewState.RaiseChanged();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
