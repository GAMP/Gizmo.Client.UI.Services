using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserViewService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        public UserViewService(UserViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

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
    }
}
