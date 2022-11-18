using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ProductDetailsPageService : ViewStateServiceBase<ProductDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ProductDetailsPageService(ProductDetailsPageViewState viewState,
            ILogger<ProductDetailsPageService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;

            _gizmoClient.GetProductsAsync(new ProductsFilter());
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadProductAsync(int id)
        {
            Random random = new Random();

            //TODO: A Load product from cache or get by id?
            var shopPageViewState = ServiceProvider.GetRequiredService<ShopPageViewState>();

            ViewState.Product = shopPageViewState?.Products?.Where(a => a.Id == id).FirstOrDefault();

            ViewState.RelatedProducts = shopPageViewState.Products.Take(5).ToList();
        }

        #endregion
    }
}