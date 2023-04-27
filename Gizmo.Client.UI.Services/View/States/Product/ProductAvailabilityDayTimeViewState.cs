using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
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
