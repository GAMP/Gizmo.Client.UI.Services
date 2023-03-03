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
        public string ProductGroupName { get; internal set; } = null!;
        public ProductType ProductType { get; internal set; }
        public string? Description { get; internal set; }
        public decimal UnitPrice { get; internal set; }
        public int? UnitPointsPrice { get; internal set; }
        public int? UnitPointsAward { get; internal set; }
        public PurchaseOptionType PurchaseOptions { get; internal set; }
        public IEnumerable<UserProductBundledViewState> BundledProducts { get; internal set; } = Enumerable.Empty<UserProductBundledViewState>();
        public int? ImageId { get; internal set; }
    }
}
