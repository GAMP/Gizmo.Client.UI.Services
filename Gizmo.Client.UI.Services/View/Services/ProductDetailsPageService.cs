using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

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
            var userCartService = ServiceProvider.GetRequiredService<UserCartService>();

            //TODO: A Load product from cache or get by id?
            var product = await _gizmoClient.GetProductByIdAsync(id);
            ViewState.Product.Id = product.Id;
            ViewState.Product.ProductGroupId = product.ProductGroupId;
            ViewState.Product.Name = product.Name;
            ViewState.Product.Description = product.Description;
            ViewState.Product.ProductType = product.ProductType;
            //TODO: A
            ViewState.Product.ImageId = null;

            ViewState.Product.CartProduct.UserCartProduct = userCartService.GetProduct(product.Id);

            var products = await _gizmoClient.GetProductsAsync(new ProductsFilter());
            ViewState.RelatedProducts = products.Data.Select(a => new ProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                ProductType = a.ProductType,
                //TODO: A Get image.
                ImageId = null
            }).Take(5).ToList();
        }

        #endregion
    }
}