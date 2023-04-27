using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserOrderViewState : ViewStateBase
    {
        #region PROPERTIES

        public int Id { get; internal set; }

        public string ProductNames { get; internal set; } = string.Empty;

        public OrderStatus OrderStatus { get; internal set; }

        public DateTime OrderDate { get; internal set; }

        public decimal TotalPrice { get; internal set; }

        public int TotalPointsPrice { get; internal set; }

        public int TotalPointsAward { get; internal set; }

        public string Notes { get; internal set; } = string.Empty;

        public IEnumerable<UserOrderLineViewState> OrderLines { get; internal set; } = Enumerable.Empty<UserOrderLineViewState>();

        public UserOrderInvoiceViewState? Invoice { get; internal set; }

        #endregion
    }
}
