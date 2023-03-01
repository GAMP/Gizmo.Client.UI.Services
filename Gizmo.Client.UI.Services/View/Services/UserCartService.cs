using System.Collections.Concurrent;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.ShopRoute)]
    public sealed class UserCartService : ValidatingViewStateServiceBase<UserCartViewState>
    {
        #region CONSTRUCTOR
        public UserCartService(
            IServiceProvider serviceProvider,
            ILogger<UserCartService> logger,
            UserCartViewState viewState,
            UserCartProductViewStateLookupService userCartProductLookupService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _userCartProductLookupService = userCartProductLookupService;
            _gizmoClient = gizmoClient;
        }

        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ConcurrentDictionary<int, int> _products = new();
        private readonly UserCartProductViewStateLookupService _userCartProductLookupService;
        #endregion

        #region FUNCTIONS

        public void SetNotes(string value)
        {
            ViewState.Notes = value;
            DebounceViewStateChange();
        }

        public void SetOrderPaymentMethod(int? paymentMethodId)
        {
            ViewState.PaymentMethodId = paymentMethodId;
            ViewState.RaiseChanged();
        }

        public UserCartProductViewState? GetProduct(int productId)
        {
            return ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault();
        }

        public async Task AddProductAsyc(int productId, int quantity = 1)
        {
            var product = await _userCartProductLookupService.GetStateAsync(productId);

            if (product is not null)
            {

                if (ViewState.UserCartProducts.ContainsKey(product.ProductId))
                {
                    ViewState.UserCartProducts[product.ProductId].Quantity += quantity;
                }
                else
                {
                    ViewState.UserCartProducts.Add(product.ProductId, product);
                }
            }
            else
            {
                ViewState.UserCartProducts.Remove(productId);
            }

            ViewState.RaiseChanged();
        }

        public Task RemoveProductAsync(int productId, int? quantity)
        {
            if (ViewState.UserCartProducts.ContainsKey(productId))
            {
                if (quantity.HasValue)
                {
                    if (ViewState.UserCartProducts[productId].Quantity >= quantity.Value)
                    {
                        ViewState.UserCartProducts[productId].Quantity -= quantity.Value;
                    }
                    else
                    {
                        ViewState.UserCartProducts.Remove(productId);
                    }
                }
                else
                {
                    ViewState.UserCartProducts.Remove(productId);
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

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                //TODO: A USER ID
                int userId = 0;
                OrderCalculateModelOptions calculateOrderOptions = new OrderCalculateModelOptions();
                var result = await _gizmoClient.UserOrderCreateAsync(userId, calculateOrderOptions);

                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                ViewState.IsComplete = true;

                ViewState.Products.Clear();
                ViewState.PaymentMethodId = null;

                ViewState.RaiseChanged();
            }
            catch
            {
                //TODO: A HANDLE ERROR
            }
            finally
            {

            }
        }

        public Task ResetAsync()
        {
            ViewState.IsComplete = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        private async Task UpdateUserCartProductsAsync()
        {
            var products = await _userCartProductLookupService.GetStatesAsync();
            ViewState.UserCartProducts = new Dictionary<int, UserCartProductViewState>(products.Count());
            // ViewState.Products = products.ToList();
            ViewState.RaiseChanged();
        }

        private async void UpdateUserCartProductsOnChangeAsync(object? _, EventArgs __) =>
            await UpdateUserCartProductsAsync();

        protected override async Task OnNavigatedIn()
        {
            _userCartProductLookupService.Changed += UpdateUserCartProductsOnChangeAsync;

            await UpdateUserCartProductsAsync();
        }
        protected override Task OnNavigatedOut()
        {
            _userCartProductLookupService.Changed -= UpdateUserCartProductsOnChangeAsync;

            return base.OnNavigatedOut();
        }
    }
}
