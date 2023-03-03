using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserCartProductItemViewState : ViewStateBase
    {
        public int ProductId { get; internal set; }
        public int Quantity { get; internal set; }
        public OrderLinePayType PayType { get; internal set; }
        public decimal TotalPrice { get; internal set; }
        public int? TotalPointsPrice { get; internal set; }
        public int? TotalPointsAward { get; internal set; }
    }
}
