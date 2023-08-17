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
            IGizmoClient gizmoClient,
            ILogger<UserRegistrationConfigurationViewService> logger,
            IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override async Task OnInitializing(CancellationToken ct)
        {
            try
            {
                //If there is no default user group this will fail.
                var userGroupDefaultRequiredInfo = await _gizmoClient.UserGroupDefaultRequiredInfoGetAsync(ct).ConfigureAwait(false);

                //just obtain the parameters on initialization, client should be connected at this point
                //we might re-query the parameters on client connection state change or change event once we have one

                ViewState.IsEnabled = await _gizmoClient.IsClientRegistrationEnabledGetAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ViewState.IsEnabled = false;
                Logger.LogError(ex, "Could not determine if registration is enabled");
            }
        }
    }
}
