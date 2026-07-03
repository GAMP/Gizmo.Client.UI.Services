using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ProductDetailsPageViewState : ViewStateBase
    {
        #region PROPERTIES

        public UserProductViewState Product { get; internal set; } = new();

        public IEnumerable<UserProductViewState> RelatedProducts { get; internal set; } = Enumerable.Empty<UserProductViewState>();

        public bool DisableProductDetails { get; internal set; }

        public bool IsShopEnabled { get; internal set; }

        /// <summary>
        /// Gets whether the product details page can be navigated to / purchased from.
        /// False when the shop is disabled or product details are disabled, in which case
        /// the page is view-only and navigation links to it should be suppressed.
        /// </summary>
        public bool ProductDetailsNavigationEnabled => IsShopEnabled && !DisableProductDetails;

        #endregion
    }
}
