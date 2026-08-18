using Gizmo.Client;
using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            IOptions<UserLoginOptions> userLoginOptions,
            ILogger<UserRegistrationConfigurationViewService> logger,
            IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userLoginOptions = userLoginOptions;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly IOptions<UserLoginOptions> _userLoginOptions;

        protected override async Task OnInitializing(CancellationToken ct)
        {
            var userLoginOptions = _userLoginOptions.Value;

            ViewState.IsDirectEnabled = userLoginOptions.IsDirectRegistrationEnabled;
            ViewState.IsPasswordRecoveryEnabled = userLoginOptions.IsPasswordRecoveryEnabled;

            try
            {
                //If there is no default user group this will fail.
                await _gizmoClient.UserGroupDefaultRequiredInfoGetAsync(ct).ConfigureAwait(false);

                ViewState.IsEnabled = userLoginOptions.IsRegistrationEnabled;
            }
            catch (Exception ex)
            {
                ViewState.IsEnabled = false;
                Logger.LogError(ex, "Could not determine if registration is enabled.");
            }
        }
    }
}
