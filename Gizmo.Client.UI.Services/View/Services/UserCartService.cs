using System.Globalization;

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

            //If current uri is not shop or product details then navigate to shop.
            var currentUri = NavigationService.GetUri();

            //TODO: A USE CONSTS?
            if (!currentUri.EndsWith("/shop") && !currentUri.Contains("/productdetails"))
                NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }
        public async Task RemoveUserCartProductAsync(int productId, int quantity = 1)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity -= quantity;

            await UpdateUserCartProductsAsync();

            productItem.RaiseChanged();
        }

        private async Task UpdateUserCartProductsAsync(CancellationToken cancellationToken = default)
        {
            var productItems = await _userCartProductItemLookupService.GetStatesAsync(cancellationToken);

            ViewState.Products = productItems.Where(x => x.Quantity > 0).ToList();

            foreach (var item in ViewState.Products)
            {
                var product = await _userProductViewStateLookupService.GetStateAsync(item.ProductId, false, cancellationToken);

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
            ValidateProperty(() => ViewState.Notes);
            DebounceViewStateChanged();
        }
        public void SetOrderPaymentMethod(int? paymentMethodId)
        {
            ViewState.PaymentMethodId = paymentMethodId;
            ValidateProperty(() => ViewState.PaymentMethodId);
            DebounceViewStateChanged();
        }
        public Task ChangeProductPayType(int productId, OrderLinePayType payType)
        {
            var existingProductState = ViewState.Products.FirstOrDefault(a => a.ProductId == productId);
            if (existingProductState != null)
            {
                existingProductState.PayType = payType;
            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }
        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                OrderCalculateModelOptions calculateOrderOptions = new OrderCalculateModelOptions();
                var result = await _gizmoClient.UserOrderCreateAsync(calculateOrderOptions);

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
        public override async Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default)
        {
            if (command.Params?.Any() != true)
                return;

            var paramProductId = command.Params.GetValueOrDefault("productId")?.ToString();

            if (paramProductId is null)
                return;

            var productId = int.Parse(paramProductId, NumberStyles.Number);

            var paramSize = command.Params.GetValueOrDefault("size")?.ToString();

            var size = 1;
            if (paramSize is not null)
                size = int.Parse(paramSize, NumberStyles.Number);

            switch (command.Type)
            {
                case ViewServiceCommandType.Add:
                    await AddUserCartProductAsync(productId, size);
                    break;
                case ViewServiceCommandType.Delete:
                    await RemoveUserCartProductAsync(productId, size);
                    break;
            }

            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await UpdateUserCartProductsAsync(cancellationToken);

            _userProductViewStateLookupService.Changed += OnUpdateUserCartProductsAsync;
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _userProductViewStateLookupService.Changed -= OnUpdateUserCartProductsAsync;

            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private async void OnUpdateUserCartProductsAsync(object? _, EventArgs __) =>
            await UpdateUserCartProductsAsync();
    }
}
