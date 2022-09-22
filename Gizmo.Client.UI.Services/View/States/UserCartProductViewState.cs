using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserCartProductViewState : ViewStateBase
    {
        #region FIELDS
        private int _productId;
        private string _productName = string.Empty;
        private int _quantity;
        private decimal _unitPrice;
        private int? _unitPointsPrice;
        private int? _unitPointsAward;
        private PurchaseOptionType _purchaseOptions;
        private OrderLinePayType _payType;
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

        public decimal UnitPrice
        {
            get { return _unitPrice; }
            internal set { SetProperty(ref _unitPrice, value); }
        }

        public int? UnitPointsPrice
        {
            get { return _unitPointsPrice; }
            internal set { SetProperty(ref _unitPointsPrice, value); }
        }

        public int? UnitPointsAward
        {
            get { return _unitPointsAward; }
            internal set { SetProperty(ref _unitPointsAward, value); }
        }

        public PurchaseOptionType PurchaseOptions
        {
            get { return _purchaseOptions; }
            internal set { SetProperty(ref _purchaseOptions, value); }
        }

        public OrderLinePayType PayType
        {
            get { return _payType; }
            internal set { SetProperty(ref _payType, value); }
        }
        public decimal TotalPrice
        {
            get
            {
                return UnitPrice * Quantity;
            }
        }

        public int? TotalPointsPrice
        {
            get
            {
                return UnitPointsPrice.HasValue ? UnitPointsPrice.Value * Quantity : null;
            }
        }

        public int? TotalUnitPointsAward
        {
            get
            {
                return UnitPointsAward.HasValue ? UnitPointsAward.Value * Quantity : null;
            }
        }

        #endregion
    }
}
