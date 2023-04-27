using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    public class ProductAvailabilityDayViewState : ViewStateBase
    {
        /// <summary>
        /// The day of the week.
        /// </summary>
        public DayOfWeek Day { get; set; }

        /// <summary>
        /// The timespans during which the product is available for this day.
        /// </summary>
        public IEnumerable<ProductAvailabilityDayTimeViewState>? DayTimesAvailable { get; set; } = Enumerable.Empty<ProductAvailabilityDayTimeViewState>();
    }
}
