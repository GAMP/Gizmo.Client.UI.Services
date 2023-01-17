using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HomePageService : ViewStateServiceBase<HomePageViewState>
    {
        #region CONSTRUCTOR
        public HomePageService(HomePageViewState viewState,
            ILogger<HomePageService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadPopularProductsAsync()
        {
            var products = await _gizmoClient.GetProductsAsync(new ProductsFilter());
            ViewState.PopularProducts = products.Data.Select(a => new ProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                ProductType = a.ProductType,
                //TODO: A Get image and user price.
                ImageId = 2, //TODO: A Default image id is not included in the product dto.
                HostGroup = "Vip",
                UnitPrice = a.Price,
                UnitPointsPrice = a.PointsPrice,
                UnitPointsAward = a.Points,
                PurchaseOptions = a.PurchaseOptions
            }).ToList();

            foreach (var product in ViewState.PopularProducts.Where(a => a.ProductType == ProductType.ProductBundle))
            {
                product.BundledProducts = new List<ProductViewState>();

                var bundledProducts = await _gizmoClient.GetBundledProductsAsync(product.Id);

                foreach (var bundledProduct in bundledProducts.Data)
                {
                    var item = await _gizmoClient.GetProductByIdAsync(bundledProduct.ProductId);

                    product.BundledProducts.Add(new ProductViewState()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        ImageId = null //TODO: A
                    });
                }
            }
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);
        }
    }
}