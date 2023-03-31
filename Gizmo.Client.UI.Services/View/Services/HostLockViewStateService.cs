using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostLockViewStateService : ViewStateServiceBase<HostLockViewState>
    {
        public HostLockViewStateService(HostLockViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<HostLockViewState> logger, 
            IServiceProvider serviceProvider)
            :base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsLocked = _gizmoClient.IsInputLocked;
            DebounceViewStateChanged();

            _gizmoClient.LockStateChange += OnLockStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LockStateChange -= OnLockStateChange;
            base.OnDisposing(isDisposing);
        }

        private void OnLockStateChange(object? sender, LockStateEventArgs e)
        {
            ViewState.IsLocked = e.IsLocked;
            DebounceViewStateChanged();
        }
    }
}
