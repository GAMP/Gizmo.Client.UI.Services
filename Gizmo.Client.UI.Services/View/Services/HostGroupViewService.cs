using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostGroupViewService : ViewStateServiceBase<HostGroupViewState>
    {
        #region CONSTRUCTOR
        public HostGroupViewService(HostGroupViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<HostGroupViewService> logger,
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
            ViewState.HostGroupId = _gizmoClient.HostGroupId;
            DebounceViewStateChanged();
            return base.OnInitializing(ct);
        }
    }
}
