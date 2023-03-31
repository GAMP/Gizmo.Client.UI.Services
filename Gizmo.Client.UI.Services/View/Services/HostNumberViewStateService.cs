using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostNumberViewStateService : ViewStateServiceBase<HostNumberViewState>
    {
        #region CONSTRUCTOR
        public HostNumberViewStateService(HostNumberViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<HostNumberViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.HostNumber = _gizmoClient.Number;
            DebounceViewStateChanged();
            return base.OnInitializing(ct);
        }

    }
}
