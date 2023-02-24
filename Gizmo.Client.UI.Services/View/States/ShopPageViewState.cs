using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ShopPageViewState : ViewStateBase
    {
        #region FIELDS
        private int? _selectedProductGroupId;
        private List<ProductGroupViewState> _productGroups = new();
        private List<ProductViewState> _products = new();
        #endregion

        #region PROPERTIES

        public int? SelectedProductGroupId
        {
            get { return _selectedProductGroupId; }
            internal set { _selectedProductGroupId = value; }
        }

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
