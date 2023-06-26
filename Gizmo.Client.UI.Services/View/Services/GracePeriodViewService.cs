using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class GracePeriodViewService : ViewStateServiceBase<GracePeriodViewState>
    {
        public GracePeriodViewService(GracePeriodViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<GracePeriodViewService> logger, 
            IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsInGracePeriod = _gizmoClient.IsInputLocked;
            DebounceViewStateChanged();

            _gizmoClient.GracePeriodChange += OnGracePeriodChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.GracePeriodChange -= OnGracePeriodChange;
            base.OnDisposing(isDisposing);
        }

        private void OnGracePeriodChange(object? sender, GracePeriodChangeEventArgs e)
        {
            ViewState.IsInGracePeriod = e.IsInGracePeriod;
            DebounceViewStateChanged();
        }
    }
}
