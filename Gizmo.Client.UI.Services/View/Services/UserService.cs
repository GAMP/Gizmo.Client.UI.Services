using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserService : ViewStateServiceBase<UserViewState>, IDisposable
    {
        public UserService(UserViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        public async Task LogοutAsync()
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
