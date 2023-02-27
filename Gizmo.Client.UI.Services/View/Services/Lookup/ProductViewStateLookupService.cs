using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ProductViewStateLookupService : ViewStateLookupServiceBase<int, ProductViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        private readonly UserCartService _userCartService;

        public ProductViewStateLookupService(
            IGizmoClient gizmoClient,
            UserCartService userCartService,
            ILogger<ProductViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userCartService = userCartService;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var products = await _gizmoClient.ProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);
            var productsDictionary = products.Data.ToDictionary(x => x.Id);
            var productGroups = await _gizmoClient.UserProductGroupsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            var productGroupsDictionary = productGroups.Data.ToDictionary(key => key.Id, value => value.Name);

            foreach (var product in products.Data)
            {
                var viewState = CreateDefaultViewState(product.Id);

                viewState.Id = product.Id;
                viewState.Name = product.Name;

                if (productGroupsDictionary.ContainsKey(product.ProductGroupId))
                    viewState.ProductGroupName = productGroupsDictionary[product.ProductGroupId];

                if (product.ProductType == ProductType.ProductBundle)
                {
                    var bundledProducts = await _gizmoClient.ProductsBundleGetAsync(product.Id, cToken);

                    if (bundledProducts.Data.Any())
                    {
                        viewState.BundledProducts = new List<ProductViewState>(bundledProducts.Data.Count());

                        foreach (var bundledProduct in bundledProducts.Data)
                        {
                            if (productsDictionary.ContainsKey(bundledProduct.ProductId))
                            {
                                var bundleProduct = productsDictionary[bundledProduct.ProductId];

                                viewState.BundledProducts.Add(new ProductViewState()
                                {
                                    Id = bundleProduct.Id,
                                    Name = bundleProduct.Name,
                                    ImageId = null
                                });
                            }
                        }
                    }
                }

                viewState.ProductGroupId = product.ProductGroupId;
                viewState.ProductType = product.ProductType;
                viewState.ImageId = null;
                viewState.HostGroup = "Vip";
                viewState.UnitPrice = product.Price;
                viewState.UnitPointsPrice = product.PointsPrice;
                viewState.PurchaseOptions = product.PurchaseOptions;

                if (product.Description is not null)
                    viewState.Description = product.Description;

                viewState.CartProduct.UserCartProduct = _userCartService.GetProduct(product.Id);

                AddViewState(product.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<ProductViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var product = await _gizmoClient.ProductGetAsync(lookUpkey, null, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (product is null)
                return viewState;

            var productGroup = await _gizmoClient.UserProductGroupGetAsync(product.ProductGroupId, cToken);

            if (product.ProductType == ProductType.ProductBundle)
            {
                var bundledProducts = await _gizmoClient.ProductsBundleGetAsync(product.Id, cToken);

                if (bundledProducts.Data.Any())
                {
                    var products = await _gizmoClient.ProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

                    var productsDictionary = products.Data.ToDictionary(x => x.Id);

                    viewState.BundledProducts = new List<ProductViewState>(bundledProducts.Data.Count());

                    foreach (var bundledProduct in bundledProducts.Data)
                    {
                        if (productsDictionary.ContainsKey(bundledProduct.ProductId))
                        {
                            var bundleProduct = productsDictionary[bundledProduct.ProductId];

                            viewState.BundledProducts.Add(new ProductViewState()
                            {
                                Id = bundleProduct.Id,
                                Name = bundleProduct.Name,
                                ImageId = null
                            });
                        }
                    }
                }
            }

            if (productGroup is not null)
                viewState.ProductGroupName = productGroup.Name;

            viewState.Id = lookUpkey;
            viewState.Name = product.Name;

            viewState.ProductGroupId = product.ProductGroupId;
            viewState.ProductType = product.ProductType;
            viewState.ImageId = null;
            viewState.HostGroup = "Vip";
            viewState.UnitPrice = product.Price;
            viewState.UnitPointsPrice = product.PointsPrice;
            viewState.PurchaseOptions = product.PurchaseOptions;

            if (product.Description is not null)
                viewState.Description = product.Description;

            viewState.CartProduct.UserCartProduct = _userCartService.GetProduct(product.Id);

            return viewState;
        }
        protected override ProductViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<ProductViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default namer";
            defaultState.Description = "Default description";
            defaultState.ProductGroupName = "Default group name";

            return defaultState;
        }
    }
}
