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
        public int? ImageId { get; internal set; }
        // public int? UnitPointsAward { get; internal set; }
        // public string? HostGroup { get; internal set; }
        // public PurchaseOptionType PurchaseOptions { get; internal set; }
        //public List<UserProductViewState>? BundledProducts { get; internal set; }
        //public UserCartProductViewState? CartProduct { get; internal set; }
    }
}
