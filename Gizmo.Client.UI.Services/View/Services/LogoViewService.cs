using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class LogoViewService : ViewStateServiceBase<LogoViewState>
    {
        public LogoViewService(LogoViewState viewState,
            IOptionsMonitor<LogoOptions> logoOptions,
            IServiceProvider serviceProvider,
            ILogger<LogoViewState> logger) : base(viewState, logger, serviceProvider)
        {
            _logoOptions = logoOptions;
        }

        private readonly IOptionsMonitor<LogoOptions> _logoOptions;

        protected override Task OnInitializing(CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(_logoOptions.CurrentValue.Logo))
            {
                ViewState.Logo = Path.Combine("https://", "static", Environment.ExpandEnvironmentVariables(_logoOptions.CurrentValue.Logo))
                    .Replace('\\', '/');
            }

            return base.OnInitializing(ct);
        }
    }
}
