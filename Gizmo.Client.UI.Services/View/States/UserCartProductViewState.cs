using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserCartProductViewState : ViewStateBase
    {
        /// <summary>
        /// Unique entry id.
        /// </summary>
        public required Guid Guid { get; init; }

        public int? ProductId { get; init; }

        public ProductType ProductEntityType { get; init; }

        public string ProductName { get; init; } = string.Empty;

        public PurchaseOptionType PurchaseOptions { get; init; }

        public int Quantity { get; internal set; }

        public OrderLinePayType PayType { get; internal set; }

        public decimal OriginalUnitPrice { get; internal set; }

        public decimal UnitPrice { get; internal set; }

        public bool IsCustomPrice { get; internal set; }

        public int? UnitPointsPrice { get; internal set; }

        public decimal TotalPrice { get; internal set; }

        public int? TotalPointsPrice { get; internal set; }

        public int? TotalPointsAward { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public long Number { get; internal set; }

        public string? Mark { get; internal set; }
    }
}
