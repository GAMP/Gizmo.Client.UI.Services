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
            UserCartProductItemViewStateLookupService userCartProductItemLookupService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _userCartProductLookupService = userCartProductLookupService;
            _userCartProductItemLookupService = userCartProductItemLookupService;
            _gizmoClient = gizmoClient;
        }

        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ConcurrentDictionary<int, int> _products = new();
        private readonly UserCartProductViewStateLookupService _userCartProductLookupService;
        private readonly UserCartProductItemViewStateLookupService _userCartProductItemLookupService;
        #endregion

        #region FUNCTIONS

        public void SetNotes(string value)
        {
            ViewState.Notes = value;
            ViewState.RaiseChanged();
        }

        public void SetOrderPaymentMethod(int? paymentMethodId)
        {
            ViewState.PaymentMethodId = paymentMethodId;
            ViewState.RaiseChanged();
        }

        public async Task<UserCartProductViewState> GetCartProductViewStateAsync(int productId) => 
            await _userCartProductLookupService.GetStateAsync(productId);
        public async Task<UserCartProductItemViewState> GetCartProductItemViewStateAsync(int productId) => 
            await _userCartProductItemLookupService.GetStateAsync(productId);

        public async Task AddProductAsync(int productId, int quantity = 1)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity += quantity;

            productItem.RaiseChanged();

            await UpdateUserCartProductsAsync();
        }

        public async Task RemoveProductAsync(int productId, int quantity = 1)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity -= quantity;

            productItem.RaiseChanged();

            await UpdateUserCartProductsAsync();
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

                ViewState.Products = Enumerable.Empty<UserCartProductViewState>();
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
            var productItems = await _userCartProductItemLookupService.GetStatesAsync();
            var products = await _userCartProductLookupService.GetStatesAsync();
            
            ViewState.Products = products.Join(productItems.Where(x => x.Quantity > 0), x => x.ProductId, y => y.ProductId, (x, _) => x);
            
            ViewState.RaiseChanged();
        }

        private async void UpdateUserCartProductsOnChangeAsync(object? _, EventArgs __) =>
            await UpdateUserCartProductsAsync();

        protected override async Task OnNavigatedIn()
        {
            await UpdateUserCartProductsAsync();
            
            _userCartProductLookupService.Changed += UpdateUserCartProductsOnChangeAsync;
        }
        protected override Task OnNavigatedOut()
        {
            _userCartProductLookupService.Changed -= UpdateUserCartProductsOnChangeAsync;

            return base.OnNavigatedOut();
        }
    }
}
