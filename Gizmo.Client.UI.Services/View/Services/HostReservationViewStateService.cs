using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Responsible of maintaining host reservation view state.
    /// </summary>
    [Register()]
    public sealed class HostReservationViewStateService : ViewStateServiceBase<HostReservationViewState>
    {
        public HostReservationViewStateService(HostReservationViewState viewState, ILogger<HostReservationViewStateService> logger, IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider) { }
    }
}
