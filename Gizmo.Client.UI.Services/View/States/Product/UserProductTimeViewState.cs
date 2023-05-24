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
        public ProductTimeUsageAvailabilityViewState? UsageAvailability { get; internal set; }

        public IEnumerable<int> DisallowedHostGroups { get; internal set; } = Enumerable.Empty<int>();

        public int ExpiresAfter { get; internal set; }

        public ProductTimeExpirationOptionType ExpirationOptions { get; internal set; }

        public ExpireFromOptionType ExpireFromOptions { get; internal set; }

        public ExpireAfterType ExpireAfterType { get; internal set; }

        public int ExpireAtDayTimeMinute { get; internal set; }

        public bool IsRestrictedForHostGroup { get; internal set; }
    }
}
