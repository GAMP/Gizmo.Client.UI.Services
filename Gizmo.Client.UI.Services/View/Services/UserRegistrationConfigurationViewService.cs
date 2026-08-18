using Gizmo.Client;
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
                await _gizmoClient.UserGroupDefaultRequiredInfoGetAsync(ct).ConfigureAwait(false);

                ViewState.IsEnabled = await _gizmoClient.IsClientRegistrationEnabledGetAsync(ct).ConfigureAwait(false);
                ViewState.IsDirectEnabled = await _gizmoClient.IsClientRegistrationDirectEnabledGetAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ViewState.IsEnabled = false;
                ViewState.IsDirectEnabled = false;
                Logger.LogError(ex, "Could not determine if registration is enabled.");
            }

            try
            {
                ViewState.IsPasswordRecoveryEnabled = await _gizmoClient.IsClientPasswordRecoveryEnabledGetAsync(ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ViewState.IsPasswordRecoveryEnabled = false;
                Logger.LogError(ex, "Could not determine if password recovery is enabled.");
            }
        }
    }
}
