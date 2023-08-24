using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class TimeProductViewState : ViewStateBase
    {
        #region PROPERTIES

        public int? UseOrder { get; internal set; }

        public UsageType TimeProductType { get; internal set; }

        public string TimeProductName { get; internal set; } = null!;

        public string Source { get; internal set; } = null!;

        public TimeSpan? Time { get; internal set; }

        public DateTime PurchaseDate { get; internal set; }

        public IEnumerable<int> AvailableHostGroups { get; internal set; } = Enumerable.Empty<int>();

        #endregion
    }
}
