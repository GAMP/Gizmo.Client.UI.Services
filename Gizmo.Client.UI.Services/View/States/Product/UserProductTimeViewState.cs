using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User product time view state.
    /// </summary>
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserProductTimeViewState : ViewStateBase
    {
        public int Minutes { get; internal set; }

        /// <summary>
        /// The usage availability of the time product.
        /// </summary>
        public ProductTimeUsageAvailabilityViewState? UsageAvailability { get; set; }

        public bool IsRestrictedForHostGroup { get; set; }
    }
}
