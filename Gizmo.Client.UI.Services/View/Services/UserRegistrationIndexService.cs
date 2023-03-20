using Gizmo.Client.UI.Services;
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
    public sealed class UserRegistrationIndexService : ViewStateServiceBase<UserRegistrationIndexViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationIndexService(UserRegistrationIndexViewState viewState,
            ILogger<UserRegistrationIndexService> logger,
            IServiceProvider serviceProvider,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        #endregion

        public async Task<bool> ProcessUserAgreements(CancellationToken cancellationToken = default)
        {
            var userAgreementsService = ServiceProvider.GetRequiredService<UserAgreementsService>();
            await userAgreementsService.LoadUserAgreementsAsync(null, cancellationToken); //TODO: AAA

            while (userAgreementsService.ViewState.CurrentUserAgreement != null)
            {
                var s = await _dialogService.ShowUserAgreementDialogAsync(cancellationToken);
                if (s.Result == DialogAddResult.Success)
                {
                    try
                    {
                        var result = await s.WaitForDialogResultAsync(cancellationToken);
                        userAgreementsService.AcceptCurrentUserAgreement();
                    }
                    catch (OperationCanceledException)
                    {
                        //TODO: A CLEANER SOLUTION?
                        if (userAgreementsService.ViewState.CurrentUserAgreement.IsRejectable)
                        {
                            userAgreementsService.RejectCurrentUserAgreement();
                        }
                        else
                        {
                            //TODO: IF REJECTED CLEANUP AND RETURN
                            userAgreementsService.RejectCurrentUserAgreement();
                            return false;
                        }
                    }
                }
            }
;
            ViewState.UserAgreementStates = userAgreementsService.ViewState.UserAgreements.Select(a => new UserAgreementModelState()
            {
                UserAgreementId = a.Id,
                AcceptState = a.AcceptState
            }).ToList();

            return true;
        }

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            //TODO: A CLEAR PREVIOUS REGISTRATION HERE?
            //UserRegistrationViewState,
            //UserRegistrationConfirmationViewState,
            //UserRegistrationConfirmationMethodViewState,
            //UserRegistrationBasicFieldsViewState,
            //UserRegistrationAdditionalFieldsViewState

            var agreementStatus = await ProcessUserAgreements(cancellationToken);

            if (agreementStatus)
            {
                //TODO: A Get ConfirmationMethod from service.
                //if (_userRegistrationViewState.ConfirmationMethod == UserRegistrationMethod.None)
                {
                    //NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                }
                //else
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
