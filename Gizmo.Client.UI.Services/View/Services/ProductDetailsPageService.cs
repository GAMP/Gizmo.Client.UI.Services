using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Web;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ProductDetailsRoute)]
    public sealed class ProductDetailsPageService : ViewStateServiceBase<ProductDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ProductDetailsPageService(ProductDetailsPageViewState viewState,
            ILogger<ProductDetailsPageService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;

            _gizmoClient.ProductsGetAsync(new ProductsFilter());
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        private async Task LoadProductAsync(int id)
        {
            var userCartService = ServiceProvider.GetRequiredService<UserCartService>();

            //TODO: A Load product from cache or get by id?

            //Test
            var product = await _gizmoClient.ProductGetAsync(id);
            ViewState.Product.Id = product.Id;
            ViewState.Product.ProductGroupId = product.ProductGroupId;
            ViewState.Product.Name = product.Name;
            ViewState.Product.Description = product.Description;
            ViewState.Product.ProductType = product.ProductType;
            //TODO: A
            ViewState.Product.ImageId = null;
            ViewState.Product.HostGroup = "Vip";
            ViewState.Product.ProductGroupName = "Beverages";

            ViewState.Product.CartProduct.UserCartProduct = userCartService.GetProduct(product.Id);

            if (ViewState.Product.ProductType == ProductType.ProductBundle)
            {
                ViewState.Product.BundledProducts = new List<ProductViewState>();

                var bundledProducts = await _gizmoClient.ProductsBundleGetAsync(product.Id);

                foreach (var bundledProduct in bundledProducts.Data)
                {
                    var item = await _gizmoClient.ProductGetAsync(bundledProduct.ProductId);

                    ViewState.Product.BundledProducts.Add(new ProductViewState()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        ImageId = null //TODO: A
                    });
                }
            }

            var products = await _gizmoClient.ProductsGetAsync(new ProductsFilter());
            ViewState.RelatedProducts = products.Data.Select(a => new ProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                ProductType = a.ProductType,
                //TODO: A Get image.
                ImageId = null,
                HostGroup = "Vip",
                ProductGroupName = "Beverages"
            }).Take(5).ToList();
            //End Test
        }

        #endregion

        protected override async Task OnNavigatedIn()
        {
            await base.OnNavigatedIn();

            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? productId = HttpUtility.ParseQueryString(uri.Query).Get("ProductId");
                if (!string.IsNullOrEmpty(productId))
                {
                    if (int.TryParse(productId, out int id))
                    {
                        await LoadProductAsync(id);
                    }
                }
            }
        }
    }
}
