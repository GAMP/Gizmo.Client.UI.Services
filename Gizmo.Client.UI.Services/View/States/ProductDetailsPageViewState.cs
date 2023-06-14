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

        #endregion
    }
}
