using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Sign in configuration view state service.
    /// </summary>
    [Register()]
    public sealed class UserLoginConfigurationViewStateService : ViewStateServiceBase<UserLoginConfigurationViewState>
    {
        public UserLoginConfigurationViewStateService(UserLoginConfigurationViewState viewState,
            ILogger<UserLoginConfigurationViewStateService> logger,
            IServiceProvider serviceProvider)
        : base(viewState, logger, serviceProvider)
        { }

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsEnabled = true;
            ViewState.IsQrLoginEnabled = true;
            return base.OnInitializing(ct);
        }
    }
}
