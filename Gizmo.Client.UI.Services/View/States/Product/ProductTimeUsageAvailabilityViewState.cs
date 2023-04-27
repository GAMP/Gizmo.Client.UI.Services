using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public class ProductTimeUsageAvailabilityViewState : ViewStateBase
    {
        /// <summary>
        /// Whether the product is available only for a specific date range.
        /// </summary>
        public bool DateRange { get; set; }

        /// <summary>
        /// The date from which the product starts to be available.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The date from which the product stops to be available.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Whether the product is available only for a specific time range within a day.
        /// </summary>
        public bool TimeRange { get; set; }

        /// <summary>
        /// The days on which the product is available.
        /// </summary>
        public IEnumerable<ProductAvailabilityDayViewState> DaysAvailable { get; set; } = Enumerable.Empty<ProductAvailabilityDayViewState>();
    }
}
