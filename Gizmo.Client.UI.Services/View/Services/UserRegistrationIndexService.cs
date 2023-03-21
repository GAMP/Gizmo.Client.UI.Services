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
                        userAgreementsService.GetNextUserAgreement();
                    }
                    catch (OperationCanceledException)
                    {
                        //TODO: A CLEANER SOLUTION?
                        if (userAgreementsService.ViewState.CurrentUserAgreement.IsRejectable)
                        {
                            userAgreementsService.SetCurrentUserAgreementState(UserAgreementAcceptState.Rejected);
                            userAgreementsService.GetNextUserAgreement();
                        }
                        else
                        {
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

        public void ClearAll()
        {
            var userRegistrationConfirmationService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationService>();
            var userRegistrationConfirmationMethodService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodService>();
            var userRegistrationBasicFieldsService = ServiceProvider.GetRequiredService<UserRegistrationBasicFieldsService>();
            var userRegistrationAdditionalFieldsService = ServiceProvider.GetRequiredService<UserRegistrationAdditionalFieldsService>();

            ViewState.UserAgreementStates = Enumerable.Empty<UserAgreementModelState>();
            userRegistrationConfirmationService.Clear();
            userRegistrationConfirmationMethodService.Clear();
            userRegistrationBasicFieldsService.Clear();
            userRegistrationAdditionalFieldsService.Clear();
        }

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ClearAll();

            var agreementStatus = await ProcessUserAgreements(cancellationToken);

            if (agreementStatus)
            {
                var userRegistrationService = ServiceProvider.GetRequiredService<UserRegistrationService>();
                var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();

                var registrationVerificationMethod = await _gizmoClient.GetRegistrationVerificationMethodAsync();

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
