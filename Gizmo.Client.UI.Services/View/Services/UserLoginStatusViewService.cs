using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginStatusViewService : ViewStateServiceBase<UserLoginStatusViewState>
    {
        public UserLoginStatusViewService(UserLoginStatusViewState viewState,
            IGizmoClient gizmoClient,
            IClientDialogService dialogService,
            ILogger<UserLoginStatusViewService> logger,
            IServiceProvider serviceProvider,
            UserChangeProfileViewService userChangeProfileViewService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
            _userChangeProfileViewService = userChangeProfileViewService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        private readonly UserChangeProfileViewService _userChangeProfileViewService;

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

        private async void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.LoginCompleted: // use login completed so the ui will only be unblocked when all login procedures have finished
                    ViewState.IsLoggedIn = true;
                    ViewState.Username = e.UserProfile?.UserName;
                    break;
                default:
                    ViewState.IsLoggedIn = false;
                    ViewState.Username = null;
                    break;
            }

            switch (e.State)
            {
                case LoginState.LoginCompleted:
                    NavigationService.NavigateTo(ClientRoutes.HomeRoute);
                    break;
                case LoginState.LoggingOut:
                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                    break;
                default:
                    break;
            }

            if (e.State == LoginState.LoginCompleted && e.IsUserPasswordRequired)
            {
                try
                {
                    var s = await _dialogService.ShowChangePasswordDialogAsync(false);
                    if (s.Result == AddComponentResultCode.Opened)
                        _ = await s.WaitForResultAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to update user password.");
                }
            }

            if (e.State == LoginState.LoginCompleted && e.IsUserInfoRequired)
            {
                try
                {
                    await _userChangeProfileViewService.StartAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to update user profile.");
                }
            }
        }
    }
}
