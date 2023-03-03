using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.HomeRoute)]
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
            //TODO: A Load popular products on page loading.

            //Test
            var products = await ((TestClient)_gizmoClient).ProductsGetAsync(new ProductsFilter());
            ViewState.PopularProducts = products.Data.Select(a => new UserProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                ProductType = a.ProductType,
                //TODO: A Get image and user price.
                ImageId = 2, //TODO: A Default image id is not included in the product dto.
                UnitPrice = a.Price,
                UnitPointsPrice = a.PointsPrice,
                UnitPointsAward = a.Points,
                PurchaseOptions = a.PurchaseOptions
            }).ToList();

            foreach (var product in ViewState.PopularProducts.Where(a => a.ProductType == ProductType.ProductBundle))
            {
                product.BundledProducts = new List<UserProductBundledViewState>();

                var bundledProducts = await ((TestClient)_gizmoClient).ProductsBundleGetAsync(product.Id);

                var tmp = new List<UserProductBundledViewState>();

                foreach (var bundledProduct in bundledProducts.Data)
                {
                    tmp.Add(new UserProductBundledViewState()
                    {
                        Id = bundledProduct.ProductId,
                        Quantity = bundledProduct.Quantity
                    });
                }

                product.BundledProducts = tmp;
            }

            foreach (var product in ViewState.PopularProducts.Where(a => a.ProductType == ProductType.ProductTime))
            {
                product.TimeProduct = new UserProductTimeViewState()
                {
                    Minutes = 45
                };
            }

            //End Test
        }

        #endregion

        protected override async Task OnNavigatedIn()
        {
            await base.OnNavigatedIn();

            await LoadPopularProductsAsync();
        }
    }
}
