using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginStatusViewStateService : ViewStateServiceBase<UserLoginStatusViewState>
    {
        public UserLoginStatusViewStateService(UserLoginStatusViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserLoginStatusViewStateService> logger,
            IServiceProvider serviceProvider):base(viewState,logger,serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

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
            switch(e.State)
            {
                case LoginState.LoggedIn:
                    ViewState.IsLoggedIn = true;
                    ViewState.Username = e.UserProfile?.UserName;
                    break;
                default:
                    ViewState.IsLoggedIn = false;
                    ViewState.Username = null;
                    break;
            }

            switch(e.State)
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
        }
    }
}
