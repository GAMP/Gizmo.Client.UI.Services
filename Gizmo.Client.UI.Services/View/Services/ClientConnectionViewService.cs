using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Responsible of maintaining client connection view state.
    /// </summary>
    [Register()]
    public sealed class ClientConnectionViewService : ViewStateServiceBase<ClientConnectionViewState>
    {
        public ClientConnectionViewService(ClientConnectionViewState viewState,
            ILogger<ClientConnectionViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsConnected = _gizmoClient.IsConnected;
            ViewState.IsConnecting = _gizmoClient.IsConnecting;

            _gizmoClient.ConnectionStateChange += OnClientConnectionStateChange;
            DebounceViewStateChanged();
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
