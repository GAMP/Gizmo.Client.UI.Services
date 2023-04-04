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
    public sealed class SignUpConfigurationViewStateService : ViewStateServiceBase<SignUpConfigurationViewState>
    {
        public SignUpConfigurationViewStateService(SignUpConfigurationViewState viewState, ILogger<SignUpConfigurationViewStateService> logger, IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        { }

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.CanSignUp = true;
            return base.OnInitializing(ct);
        }
    }
}
