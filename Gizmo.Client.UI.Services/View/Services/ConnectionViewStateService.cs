using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ConnectionViewStateService : ViewStateServiceBase<ConnectionViewState>
    {
        public ConnectionViewStateService(ConnectionViewState viewState,
            ILogger<ConnectionViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.ConnectionStateChange += OnClientConnectionStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.ConnectionStateChange -= OnClientConnectionStateChange;
            base.OnDisposing(isDisposing);
        }

        private void OnClientConnectionStateChange(object? sender, ConnectionStateEventArgs e)
        {
            ViewState.IsConnecting = e.IsConnecting;
            ViewState.IsConnected = e.IsConnected;
            DebounceViewStateChanged();
        }
    }

}
