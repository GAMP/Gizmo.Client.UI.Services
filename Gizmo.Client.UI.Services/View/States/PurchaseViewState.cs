using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class PurchaseViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private ProductType _productType;
        private string _productName = string.Empty;
        private OrderStatus _orderStatus;
        private int _quantity;
        private decimal _total;
        private string _paymentMethod = string.Empty;
        private DateTime _orderDate;
        private List<ProductViewState> _bundledProducts = new();
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }
        }

        public ProductType ProductType
        {
            get { return _productType; }
            internal set { SetProperty(ref _productType, value); }
        }

        public string ProductName
        {
            get { return _productName; }
            internal set { SetProperty(ref _productName, value); }
        }

        public OrderStatus OrderStatus
        {
            get { return _orderStatus; }
            internal set { SetProperty(ref _orderStatus, value); }
        }

        public int Quantity
        {
            get { return _quantity; }
            internal set { SetProperty(ref _quantity, value); }
        }

        public decimal Total
        {
            get { return _total; }
            internal set { _total = value; }
        }

        public string PaymentMethod
        {
            get { return _paymentMethod; }
            internal set { _paymentMethod = value; }
        }

        public DateTime OrderDate
        {
            get { return _orderDate; }
            internal set { _orderDate = value; }
        }

        public List<ProductViewState> BundledProducts
        {
            get { return _bundledProducts; }
            internal set { SetProperty(ref _bundledProducts, value); }
        }

        #endregion
    }
}
