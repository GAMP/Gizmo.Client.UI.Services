﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ShopPageService : ViewStateServiceBase<ShopPageViewState>
    {
        #region CONSTRUCTOR
        public ShopPageService(
            ShopPageViewState viewState,
            ProductGroupViewStateLookupService productGroupService,
            ILogger<ShopPageService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _productGroupService = productGroupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ProductGroupViewStateLookupService _productGroupService;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadProductsAsync()
        {
            var userCartService = ServiceProvider.GetRequiredService<UserCartService>();

            var products = await ((TestClient)_gizmoClient).ProductsGetAsync(new ProductsFilter() { ProductGroupId = ViewState.SelectedProductGroupId });
            ViewState.Products = products.Data.Select(a => new ProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                ProductType = a.ProductType,
                //TODO: A Get image and user price.
                ImageId = null, //TODO: A Default image id is not included in the product dto.,
                HostGroup = "Vip",
                UnitPrice = a.Price,
                UnitPointsPrice = a.PointsPrice,
                UnitPointsAward = a.Points,
                PurchaseOptions = a.PurchaseOptions,
                ProductGroupName = "Beverages"
            }).ToList();

            foreach (var product in ViewState.Products)
            {
                if (product.ProductType == ProductType.ProductBundle)
                {
                    product.BundledProducts = new List<ProductViewState>();

                    var bundledProducts = await ((TestClient)_gizmoClient).ProductsBundleGetAsync(product.Id);

                    foreach (var bundledProduct in bundledProducts.Data)
                    {
                        var item = await ((TestClient)_gizmoClient).ProductGetAsync(bundledProduct.ProductId);

                        product.BundledProducts.Add(new ProductViewState()
                        {
                            Id = item.Id,
                            Name = item.Name,
                            ImageId = null //TODO: A
                        });
                    }
                }

                product.CartProduct.UserCartProduct = userCartService.GetProduct(product.Id);
            }

            ViewState.RaiseChanged();
        }

        public async Task SetSelectedProductGroup(int? selectedProductGroupId)
        {
            ViewState.SelectedProductGroupId = selectedProductGroupId;

            await LoadProductsAsync();

            ViewState.RaiseChanged();
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken cToken)
        {
            await base.OnInitializing(cToken);

            var productGroups = await _productGroupService.GetAsync(cToken);

            ViewState.ProductGroups = productGroups.ToList();
        }
    }
}
