using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class ProductViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private ProductType _productType;
        private int _productGroupId;
        private string _name = string.Empty;
        private string _description = string.Empty;
        private decimal _unitPrice;
        private int? _unitPointsPrice;
        private int? _unitPointsAward;
        private string _image = string.Empty;
        private PurchaseOptionType _purchaseOptions;
        private List<ProductViewState> _bundledProducts;
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

        public int ProductGroupId
        {
            get { return _productGroupId; }
            internal set { SetProperty(ref _productGroupId, value); }
        }

        public string Name
        {
            get { return _name; }
            internal set { SetProperty(ref _name, value); }
        }

        public string Description
        {
            get { return _description; }
            internal set { SetProperty(ref _description, value); }
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

        public string Image
        {
            get { return _image; }
            internal set { SetProperty(ref _image, value); }
        }

        public PurchaseOptionType PurchaseOptions
        {
            get { return _purchaseOptions; }
            internal set { SetProperty(ref _purchaseOptions, value); }
        }

        public List<ProductViewState> BundledProducts
        {
            get { return _bundledProducts; }
            internal set { SetProperty(ref _bundledProducts, value); }
        }

        #endregion
    }
}
