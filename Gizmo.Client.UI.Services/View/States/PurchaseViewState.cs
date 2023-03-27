using Gizmo.UI.View.States;
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
        private IEnumerable<UserProductViewState> _bundledProducts = Enumerable.Empty<UserProductViewState>();
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        public ProductType ProductType
        {
            get { return _productType; }
            internal set { _productType = value; }
        }

        public string ProductName
        {
            get { return _productName; }
            internal set { _productName = value; }
        }

        public OrderStatus OrderStatus
        {
            get { return _orderStatus; }
            internal set { _orderStatus = value; }
        }

        public int Quantity
        {
            get { return _quantity; }
            internal set { _quantity = value; }
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

        public IEnumerable<UserProductViewState> BundledProducts
        {
            get { return _bundledProducts; }
            internal set { _bundledProducts = value; }
        }

        #endregion
    }
}
