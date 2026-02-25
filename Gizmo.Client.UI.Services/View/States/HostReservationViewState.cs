using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
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

        public int? ReservationId { get; internal set; }

        public DateTime? Time { get; internal set; }

        public int? Duration { get; internal set; }

        public decimal? Total { get; internal set; }

        public decimal? Outstanding { get; internal set; }

        public IEnumerable<ReservationInfoHostModel> Hosts { get; internal set; } = [];

        public bool ReservationNotificationTimeReached { get; internal set; }

        public bool ReservationBlockTimeReached { get; internal set; }

        public bool ReservationTimeReached { get; internal set; }

        public ReservationPaymentStatus? ReservationPaymentStatus { get; internal set; }

        public bool Ignored { get; internal set; }

        public DateTime? DismissedTime { get; internal set; }

        #endregion
    }
}
