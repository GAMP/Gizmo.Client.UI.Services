using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Sign up configuration view state service.
    /// </summary>
    [Register()]
    public sealed class UserRegistrationConfigurationViewService : ViewStateServiceBase<UserRegistrationConfigurationViewState>
    {
        public UserRegistrationConfigurationViewService(UserRegistrationConfigurationViewState viewState,
            IServerInfoService serverInfo,
            ILogger<UserRegistrationConfigurationViewService> logger,
            IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        {
            _serverInfo = serverInfo;
        }

        private readonly IServerInfoService _serverInfo;

        protected override async Task OnInitializing(CancellationToken ct)
        {
            try
            {
                //just obtain the parameters on initialization, client should be connected at this point
                //we might re-query on connection/state change once we have one
                ViewState.IsEnabled = await _serverInfo.GetRegistrationEnabledAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ViewState.IsEnabled = false;
                Logger.LogError(ex, "Could not determine if registration is enabled");
            }
        }
    }
}
