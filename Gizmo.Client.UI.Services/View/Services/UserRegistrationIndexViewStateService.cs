﻿using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationIndexRoute)]
    public sealed class UserRegistrationIndexViewStateService : ViewStateServiceBase<UserRegistrationIndexViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationIndexViewStateService(UserRegistrationIndexViewState viewState,
            ILogger<UserRegistrationIndexViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        #endregion

        public async Task<bool> ProcessUserAgreements(CancellationToken cancellationToken = default)
        {
            var userAgreements = await _gizmoClient.UserAgreementsGetAsync(new UserAgreementsFilter(), cancellationToken);

            var userAgreementStates = userAgreements.Data.Select(a => new UserAgreementViewState()
            {
                Id = a.Id,
                Name = a.Name,
                Agreement = a.Agreement,
                IsRejectable = a.IsRejectable,
                IgnoreState = a.IgnoreState,
                AcceptState = UserAgreementAcceptState.None
            }).ToList();

            foreach (var userAgreement in userAgreementStates)
            {
                var s = await _dialogService.ShowUserAgreementDialogAsync(new UserAgreementDialogParameters()
                {
                    Name = userAgreement.Name,
                    Agreement = userAgreement.Agreement,
                    IsRejectable = userAgreement.IsRejectable
                }, cancellationToken);
                if (s.Result == DialogAddResult.Success)
                {
                    try
                    {
                        var result = await s.WaitForDialogResultAsync(cancellationToken);
                        if (result.Accepted)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Accepted;
                        }
                        else
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Rejected;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        if (userAgreement.IsRejectable)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Rejected;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            ViewState.UserAgreementStates = userAgreementStates;

            return true;
        }

        public void ClearAll()
        {
            var userRegistrationConfirmationService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationViewStateService>();
            var userRegistrationConfirmationMethodService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewStateService>();
            var userRegistrationBasicFieldsService = ServiceProvider.GetRequiredService<UserRegistrationBasicFieldsViewStateService>();
            var userRegistrationAdditionalFieldsService = ServiceProvider.GetRequiredService<UserRegistrationAdditionalFieldsViewStateService>();

            userRegistrationConfirmationService.Clear();
            userRegistrationConfirmationMethodService.Clear();
            userRegistrationBasicFieldsService.Clear();
            userRegistrationAdditionalFieldsService.Clear();

            ViewState.UserAgreementStates = Enumerable.Empty<UserAgreementViewState>();
            DebounceViewStateChanged();
        }

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ClearAll();

            var agreementStatus = await ProcessUserAgreements(cancellationToken);

            if (agreementStatus)
            {
                var userRegistrationService = ServiceProvider.GetRequiredService<UserRegistrationViewStateService>();
                var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();

                var registrationVerificationMethod = await _gizmoClient.RegistrationVerificationMethodGetAsync();

                userRegistrationService.SetConfirmationMethod(registrationVerificationMethod);

                if (userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.None)
                {
                    NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                }
                else
                {
                    NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationMethodRoute);
                }
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.LoginRoute);
            }
        }

        #endregion
    }
}
