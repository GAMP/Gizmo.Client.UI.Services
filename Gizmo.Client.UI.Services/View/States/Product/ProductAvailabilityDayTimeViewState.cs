using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    public class ProductAvailabilityDayTimeViewState : ViewStateBase
    {
        /// <summary>
        /// The start second of this timespan.
        /// </summary>
        public int StartSecond { get; set; }

        /// <summary>
        /// The end second of this timespan.
        /// </summary>
        public int EndSecond { get; set; }
    }
}
