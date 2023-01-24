using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserCartService : ValidatingViewStateServiceBase<UserCartViewState>
    {
        #region CONSTRUCTOR
        public UserCartService(UserCartViewState viewState,
            ILogger<UserCartService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ConcurrentDictionary<int, int> _products = new();
        #endregion

        #region PROPERTIES


        #endregion

        #region FUNCTIONS

        public void SetNotes(string value)
        {
            ViewState.Notes = value;
            ViewState.RaiseChanged();
        }

        public UserCartProductViewState? GetProduct(int productId)
        {
            return ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault();
        }

        public async Task AddProductAsyc(int productId, int quantity = 1)
        {
            Random random = new Random();

            var existingProductViewState = ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault();
            if (existingProductViewState != null)
            {
                existingProductViewState.Quantity += quantity;

                existingProductViewState.RaiseChanged();
            }
            else
            {
                var shopPageViewState = ServiceProvider.GetRequiredService<ShopPageViewState>();
                var productDetailsPageService = ServiceProvider.GetRequiredService<ProductDetailsPageService>();

                var product = await _gizmoClient.GetProductByIdAsync(productId);

                if (product != null)
                {
                    var productViewState = ServiceProvider.GetRequiredService<UserCartProductViewState>();
                    productViewState.Quantity = quantity;
                    productViewState.ProductId = product.Id;
                    productViewState.ProductName = product.Name;
                    //TODO: A Get user price.
                    productViewState.UnitPrice = product.Price;
                    productViewState.UnitPointsPrice = product.PointsPrice;
                    productViewState.UnitPointsAward = product.Points;
                    productViewState.PurchaseOptions = product.PurchaseOptions;
                    //TODO: A CALCULATE PAY TYPE
                    productViewState.PayType = productViewState.PurchaseOptions == PurchaseOptionType.And ? OrderLinePayType.Mixed : OrderLinePayType.Cash;

                    ViewState.Products.Add(productViewState);

                    var shopProduct = shopPageViewState.Products.Where(a => a.Id == productId).FirstOrDefault();
                    if (shopProduct != null)
                    {
                        shopProduct.CartProduct.UserCartProduct = productViewState;
                        shopProduct.CartProduct.RaiseChanged();
                    }

                    if (productDetailsPageService.ViewState.Product != null && productDetailsPageService.ViewState.Product.Id == productId)
                    {
                        productDetailsPageService.ViewState.Product.CartProduct.UserCartProduct = productViewState;
                        productDetailsPageService.ViewState.Product.CartProduct.RaiseChanged();
                    }
                }
                else
                {
                    //TODO: A ERROR?
                }
            }

            ViewState.RaiseChanged();

            //If current uri is not shop or product details then navigate to shop.
            var currentUri = NavigationService.GetUri();

            //TODO: A USE CONSTS?
            if (!currentUri.EndsWith("/shop") && !currentUri.Contains("/productdetails/"))
                NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }

        public Task RemoveProductAsync(int productId, int? quantity)
        {
            var existingProductState = ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault();
            if (existingProductState != null)
            {
                if (quantity.HasValue && quantity.Value < existingProductState.Quantity)
                {
                    existingProductState.Quantity -= quantity.Value;
                }
                else
                {
                    ViewState.Products.Remove(existingProductState);
                }
            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task DeleteProduct(int productId)
        {
            if (_products.TryGetValue(productId, out var product))
            {
            }
            return Task.CompletedTask;
        }

        public Task ChangeProductPayType(int productId, OrderLinePayType payType)
        {
            var existingProductState = ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault();
            if (existingProductState != null)
            {
                existingProductState.PayType = payType;
            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task<bool> IsProductInCartAync(int productId)
        {
            return Task.FromResult(_products.ContainsKey(productId));
        }

        public async ValueTask<bool> TryGetProductViewStateAsync(int productId)
        {
            if (_products.TryGetValue(productId, out var productViewState))
                return true;

            await Task.Delay(1000);

            return false;
        }

        public void SetOrderPaymentMethod(int? paymentMethodId)
        {
            ViewState.PaymentMethodId = paymentMethodId;

            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                ViewState.IsComplete = true;
                ViewState.RaiseChanged();
            }
            catch
            {

            }
            finally
            {

            }
        }

        public Task ResetAsync()
        {
            ViewState.Products.Clear();
            ViewState.PaymentMethodId = null;
            ViewState.IsComplete = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion
    }
}