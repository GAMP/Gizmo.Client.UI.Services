using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ProductDetailsPageViewState : ViewStateBase
    {
        #region FIELDS
        private ProductViewState _product = new();
        private List<ProductViewState> _relatedProducts = new();
        #endregion

        #region PROPERTIES

        public ProductViewState Product
        {
            get { return _product; }
            internal set { _product = value; }
        }

        public List<ProductViewState> RelatedProducts
        {
            get { return _relatedProducts; }
            internal set { _relatedProducts = value; }
        }

        #endregion
    }
}
