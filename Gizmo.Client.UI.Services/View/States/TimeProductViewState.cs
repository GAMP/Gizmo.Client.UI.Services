using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class TimeProductViewState : ViewStateBase
    {
        #region PROPERTIES

        public int? ActivationOrder { get; internal set; } = null!;

        public UsageType TimeProductType { get; internal set; }

        public string TimeProductName { get; internal set; } = null!;

        public string Source { get; internal set; } = null!;

        public string Time { get; internal set; } = null!;

        public DateTime? PurchaseDate { get; internal set; }

        public DateTime? ExpirationDate { get; internal set; }

        public IEnumerable<int> AvailableHostGroups { get; internal set; } = Enumerable.Empty<int>();

        public int? ProductId { get; internal set; }

        public bool InCredit { get; internal set; }

        #endregion
    }
}
