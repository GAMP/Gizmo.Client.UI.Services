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
            UserProductViewStateLookupService userProductViewStateLookupService,
            UserCartProductItemViewStateLookupService userCartProductItemLookupService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _userCartProductItemLookupService = userCartProductItemLookupService;
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly UserCartProductItemViewStateLookupService _userCartProductItemLookupService;
        #endregion

        #region FUNCTIONS
        public async Task<UserProductViewState> GetCartProductViewStateAsync(int productId) =>
           await _userProductViewStateLookupService.GetStateAsync(productId);
        public async Task<UserCartProductItemViewState> GetCartProductItemViewStateAsync(int productId) =>
            await _userCartProductItemLookupService.GetStateAsync(productId);

        public async Task AddUserCartProductAsync(int productId, int quantity = 1)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity += quantity;

            await UpdateUserCartProductsAsync();

            productItem.RaiseChanged();
        }
        public async Task RemoveUserCartProductAsync(int productId, int quantity = 1)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity -= quantity;

            await UpdateUserCartProductsAsync();

            productItem.RaiseChanged();
        }

        private async Task UpdateUserCartProductsAsync()
        {
            var productItems = await _userCartProductItemLookupService.GetStatesAsync();

            ViewState.Products = productItems.Where(x => x.Quantity > 0).ToList();

            foreach (var item in ViewState.Products)
            {
                var product = await _userProductViewStateLookupService.GetStateAsync(item.ProductId);

                item.TotalPrice = product.UnitPrice * item.Quantity;
                item.TotalPointsPrice = product.UnitPointsPrice * item.Quantity;
                item.TotalPointsAward = product.UnitPointsAward * item.Quantity;
            }

            ViewState.Total = ViewState.Products.Where(a => a.PayType == OrderLinePayType.Cash || a.PayType == OrderLinePayType.Mixed).Select(a => a.TotalPrice).Sum();
            ViewState.PointsTotal = ViewState.Products.Where(a => a.PayType == OrderLinePayType.Points || a.PayType == OrderLinePayType.Mixed).Select(a => (a.TotalPointsPrice ?? 0)).Sum();
            ViewState.PointsAward = ViewState.Products.Select(a => (a.TotalPointsAward ?? 0)).Sum();

            ViewState.RaiseChanged();
        }

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

                ViewState.Products = Enumerable.Empty<UserCartProductItemViewState>();
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

        protected override async Task OnNavigatedIn()
        {
            await UpdateUserCartProductsAsync();

            _userProductViewStateLookupService.Changed += OnUpdateUserCartProductsAsync;
        }
        protected override Task OnNavigatedOut()
        {
            _userProductViewStateLookupService.Changed -= OnUpdateUserCartProductsAsync;

            return base.OnNavigatedOut();
        }
        private async void OnUpdateUserCartProductsAsync(object? _, EventArgs __) =>
            await UpdateUserCartProductsAsync();
    }
}
