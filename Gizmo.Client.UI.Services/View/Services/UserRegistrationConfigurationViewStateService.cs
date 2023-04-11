using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Sign up configuration view state service.
    /// </summary>
    [Register()]
    public sealed class UserRegistrationConfigurationViewStateService : ViewStateServiceBase<UserRegistrationConfigurationViewState>
    {
        public UserRegistrationConfigurationViewStateService(UserRegistrationConfigurationViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserRegistrationConfigurationViewStateService> logger,
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
                //just obtain the parameters on initialization, client should be connected at this point
                //we might re-query the parameters on client connection state change or change event once we have one

                var registrationMethod = await _gizmoClient.RegistrationVerificationMethodGetAsync(ct).ConfigureAwait(false);
                ViewState.IsEnabled = registrationMethod != RegistrationVerificationMethod.None;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not determine if registration is enabled");
            }
        }
    }
}
