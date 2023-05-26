using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User product view state.
    /// </summary>
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserProductViewState : UserProductViewStateBase
    {
        public string Name { get; internal set; } = null!;
        public int ProductGroupId { get; internal set; }
        public ProductType ProductType { get; internal set; }
        public string? Description { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public int? UnitPointsPrice { get; internal set; }
        public int? UnitPointsAward { get; internal set; }
        public PurchaseOptionType PurchaseOptions { get; internal set; }
        public IEnumerable<UserProductBundledViewState> BundledProducts { get; internal set; } = Enumerable.Empty<UserProductBundledViewState>();
        public UserProductTimeViewState TimeProduct { get; internal set; } = null!;
        public int? DefaultImageId { get; internal set; }

        /// <summary>
        /// The usage availability of the time product.
        /// </summary>
        public ProductPurchaseAvailabilityViewState? PurchaseAvailability { get; set; }

        /// <summary>
        /// Whether the product has enabled stock control and disallow sale out of stock.
        /// </summary>
        public bool IsStockLimited { get; internal set; }

        /// <summary>
        /// Whether the product is restricted for guest and current user is guest.
        /// </summary>
        public bool IsRestrictedForGuest { get; internal set; }

        /// <summary>
        /// Whether the product is restricted for current user group.
        /// </summary>
        public bool IsRestrictedForUserGroup { get; internal set; }

        /// <summary>
        /// The list of host group where this product is hidden.
        /// </summary>
        public IEnumerable<int> HiddenHostGroups { get; internal set; } = Enumerable.Empty<int>();

        /// <summary>
        /// Whether the product is restricted for current user.
        /// </summary>
        public bool DisallowPurchase { get; internal set; }

        /// <summary>
        /// The reason for this product to be restricted for current user.
        /// </summary>
        public string DisallowPurchaseReason { get; internal set; } = null!;

        /// <summary>
        /// Whether the product is restricted for the moment.
        /// </summary>
        public bool DisallowUse { get; internal set; }
    }
}
