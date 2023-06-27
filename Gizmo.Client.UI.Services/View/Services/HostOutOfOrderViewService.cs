using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostOutOfOrderViewService : ViewStateServiceBase<HostOutOfOrderViewState>
    {
        public HostOutOfOrderViewService(HostOutOfOrderViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<HostOutOfOrderViewService> logger,
            IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {            
            ViewState.IsOutOfOrder = _gizmoClient.IsOutOfOrder;
            DebounceViewStateChanged();

            _gizmoClient.OutOfOrderStateChange += OnOutOfOrderStateChange;

            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.OutOfOrderStateChange -= OnOutOfOrderStateChange;
            base.OnDisposing(isDisposing);
        }

        private void OnOutOfOrderStateChange(object? sender, OutOfOrderStateEventArgs e)
        {
            ViewState.IsOutOfOrder = e.IsOutOfOrder;
            DebounceViewStateChanged();
        }
    }
}
