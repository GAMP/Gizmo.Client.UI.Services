using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostOutOfOrderViewStateService : ViewStateServiceBase<HostOutOfOrderViewState>
    {
        public HostOutOfOrderViewStateService(HostOutOfOrderViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<HostOutOfOrderViewState> logger,
            IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {            
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
        }
    }
}
