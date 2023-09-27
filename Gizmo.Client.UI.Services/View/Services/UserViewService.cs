using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserViewService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        public UserViewService(UserViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserViewService> logger,
            IServiceProvider serviceProvider,
            IClientDialogService dialogService,
            ILocalizationService localizationService,
            IOptions<ClientInterfaceOptions> clientInterfaceOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
            _localizationService = localizationService;
            _clientInterfaceOptions = clientInterfaceOptions;
        }

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        private readonly ILocalizationService _localizationService;
        private readonly IOptions<ClientInterfaceOptions> _clientInterfaceOptions;
        #endregion

        public async Task LogoutWithConfirmationAsync()
        {
            try
            {
                var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_LOGOUT_VERIFICATION_TITLE"), _localizationService.GetString("GIZ_LOGOUT_VERIFICATION_MESSAGE"), AlertDialogButtons.YesNo);
                if (s.Result == AddComponentResultCode.Opened)
                {
                    var result = await s.WaitForResultAsync();

                    if (s.Result == AddComponentResultCode.Ok && result!.Button == AlertDialogResultButton.Yes)
                        await _gizmoClient.UserLogoutAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated logout failed.");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _gizmoClient.UserLogoutAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated logout failed.");
            }
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnUserLoginStateChange;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);

            _gizmoClient.LoginStateChange += OnUserLoginStateChange;
        }

        private void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoginCompleted)
            {
                ViewState.Id = e.UserProfile.Id;
                if (e.UserProfile.IsGuest)
                {
                    ViewState.Username = _localizationService.GetString("GIZ_GEN_GUEST");
                }
                else
                {
                    ViewState.Username = e.UserProfile.UserName;
                }
                ViewState.IsUserLogoutEnabled = _gizmoClient.IsUserLogoutEnabled;
                ViewState.IsUserLockDisabled = _clientInterfaceOptions.Value.DisableUserLock;
                ViewState.IsGuest = e.UserProfile.IsGuest;

                DebounceViewStateChanged();
            }
            else if (e.State == LoginState.LoggingOut)
            {
                ViewState.Id = 0;
                ViewState.Username = null;
                ViewState.IsUserLogoutEnabled = false;
                ViewState.IsUserLockDisabled = true;
                ViewState.IsGuest = false;

                DebounceViewStateChanged();
            }
        }
    }
}
