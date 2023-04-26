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
    public sealed class UserLoginConfigurationViewService : ViewStateServiceBase<UserLoginConfigurationViewState>
    {
        public UserLoginConfigurationViewService(UserLoginConfigurationViewState viewState,
            ILogger<UserLoginConfigurationViewService> logger,
            IServiceProvider serviceProvider)
        : base(viewState, logger, serviceProvider)
        { }

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsEnabled = true;
            return base.OnInitializing(ct);
        }
    }
}
