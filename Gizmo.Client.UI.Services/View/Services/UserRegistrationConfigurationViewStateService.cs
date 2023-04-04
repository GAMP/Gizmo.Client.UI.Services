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
        public UserRegistrationConfigurationViewStateService(UserRegistrationConfigurationViewState viewState, ILogger<UserRegistrationConfigurationViewStateService> logger, IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        { }

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsEnabled = true;
            return base.OnInitializing(ct);
        }
    }
}
