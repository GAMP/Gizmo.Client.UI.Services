using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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

        public Task AddProductAsyc(int productId, int quantity = 1)
        {
            Random random = new Random();

            var existingProductViewState = ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault();
            if (existingProductViewState != null)
            {
                existingProductViewState.Quantity += quantity;
            }
            else
            {
                var shopPageViewState = ServiceProvider.GetRequiredService<ShopPageViewState>();

                var product = shopPageViewState.Products.Where(a => a.Id == productId).FirstOrDefault();

                if (product != null)
                {
                    var productViewState = ServiceProvider.GetRequiredService<UserCartProductViewState>();
                    productViewState.Quantity = quantity;
                    productViewState.ProductId = product.Id;
                    productViewState.ProductName = product.Name;
                    productViewState.UnitPrice = product.UnitPrice;
                    productViewState.UnitPointsPrice = product.UnitPointsPrice;
                    productViewState.UnitPointsAward = product.UnitPointsAward;
                    productViewState.PurchaseOptions = product.PurchaseOptions;
                    productViewState.PayType = productViewState.PurchaseOptions == PurchaseOptionType.And ? OrderLinePayType.Mixed : OrderLinePayType.Cash;
                    //TODO: A CALCULATE PAY TYPE

                    ViewState.Products.Add(productViewState);
                }
                else
                {
                    //TODO: A ERROR?
                }
            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
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

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            //TODO: A SEND ORDER TO SERVER

            ViewState.IsComplete = true;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
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