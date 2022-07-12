using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserCartProductViewState : ViewStateBase
    {
        #region FIELDS
        private string _productName = string.Empty;
        private int _quantity;
        private int _productId;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets product id.
        /// </summary>
        /// <remarks>
        /// The value is intended to be used as a item key to speed up rendering and executing commands on this product.
        /// </remarks>
        public int ProductId
        {
            get { return _productId; }
            internal set { SetProperty(ref _productId, value); }        
        }

        /// <summary>
        /// Gets product name.
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            internal set { SetProperty(ref _productName, value); }
        }

        /// <summary>
        /// Gets current product quantity.
        /// </summary>
        public int Quantity
        {
            get { return _quantity; }
            internal set { SetProperty(ref _quantity, value); }
        } 
        
        #endregion
    }
}
