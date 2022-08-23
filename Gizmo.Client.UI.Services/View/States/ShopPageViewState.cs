using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ShopPageViewState : ViewStateBase
    {
        #region FIELDS
        private List<ProductGroupViewState> _productGroups = new();
        private List<ProductViewState> _products = new();
        #endregion

        #region PROPERTIES

        public List<ProductGroupViewState> ProductGroups
        {
            get { return _productGroups; }
            internal set { _productGroups = value; }
        }

        public List<ProductViewState> Products
        {
            get { return _products; }
            internal set { _products = value; }
        }

        #endregion
    }
}
