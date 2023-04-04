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
    public sealed class SignInConfigurationViewStateService : ViewStateServiceBase<SignInConfigurationViewState>
    {
        public SignInConfigurationViewStateService(SignInConfigurationViewState viewState,
            ILogger<SignInConfigurationViewStateService> logger,
            IServiceProvider serviceProvider)
        : base(viewState, logger, serviceProvider)
        { }

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.CanSignIn = true;
            ViewState.CanSignInWithQR = true;
            return base.OnInitializing(ct);
        }
    }
}
