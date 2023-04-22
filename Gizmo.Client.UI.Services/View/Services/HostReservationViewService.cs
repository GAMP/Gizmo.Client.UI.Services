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
    public sealed class HostReservationViewService : ViewStateServiceBase<HostReservationViewState>
    {
        public HostReservationViewService(HostReservationViewState viewState, ILogger<HostReservationViewService> logger, IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider) { }
    }
}
