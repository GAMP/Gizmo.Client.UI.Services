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
    public sealed class UserRegistrationIndexViewService : ViewStateServiceBase<UserRegistrationIndexViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationIndexViewService(UserRegistrationIndexViewState viewState,
            ILogger<UserRegistrationIndexViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IClientDialogService dialogService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _dialogService = dialogService;
            _registrationSession = registrationSession;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IClientDialogService _dialogService;
        private readonly IRegistrationSessionService _registrationSession;
        #endregion

        public async Task<bool> ProcessUserAgreements(IReadOnlyList<RegistrationAgreement> agreements, CancellationToken cancellationToken = default)
        {
            if (!agreements.Any())
            {
                _registrationSession.SetAgreementsAccepted(true);
                return true;
            }

            var addDialogResult = await _dialogService.ShowRegistrationAgreementsDialogAsync(agreements, cancellationToken);

            if (addDialogResult.Result == AddComponentResultCode.Opened)
            {
                var componentResult = await addDialogResult.WaitForResultAsync(cancellationToken);
                if (addDialogResult.Result == AddComponentResultCode.Ok && componentResult?.AllMandatoryAccepted == true)
                {
                    _registrationSession.SetAgreementsAccepted(true);
                    return true;
                }
            }

            return false; // Dismissed / Canceled / not accepted
        }

        public void ClearAll()
        {
            _registrationSession.Clear();
            ViewState.UserAgreementStates = Enumerable.Empty<UserAgreementViewState>();
            DebounceViewStateChanged();
        }

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ClearAll();

            try
            {
                var providers = await _registrationService.GetProvidersAsync(cancellationToken);
                var agreements = await _registrationService.GetAgreementsAsync(cancellationToken);

                var agreementStatus = await ProcessUserAgreements(agreements, cancellationToken);

                if (agreementStatus)
                {
                    var userGroupDefaultRequiredInfo = await _registrationService.GetRequiredUserInfoAsync(cancellationToken);

                    _registrationSession.SetRequiredUserInfo(userGroupDefaultRequiredInfo);

                    if (providers.Count > 0)
                    {
                        NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                    }
                    else
                    {
                        NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                    }
                }
                else
                {
                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration service error.");
            }
        }

        #endregion
    }
}
