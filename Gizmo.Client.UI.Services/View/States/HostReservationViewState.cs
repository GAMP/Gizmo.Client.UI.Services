using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Host reservation view state.
    /// </summary>
    [Register()]
    public sealed class HostReservationViewState : ViewStateBase
    {
        #region PROPERTIES

        public DateTime? Time { get; internal set; }

        public bool ReservationNotificationTimeReached { get; internal set; }

        public bool ReservationBlockTimeReached { get; internal set; }

        #endregion
    }
}
