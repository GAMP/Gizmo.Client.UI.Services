using System.Globalization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.ShopRoute)]
    public sealed class UserCartViewService : ValidatingViewStateServiceBase<UserCartViewState>
    {
        #region CONSTRUCTOR
        public UserCartViewService(
            IServiceProvider serviceProvider,
            ILogger<UserCartViewService> logger,
            UserCartViewState viewState,
            UserProductViewStateLookupService userProductViewStateLookupService,
            UserCartProductItemViewStateLookupService userCartProductItemLookupService,
            IClientDialogService dialogService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _userCartProductItemLookupService = userCartProductItemLookupService;
            _dialogService = dialogService;
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly UserCartProductItemViewStateLookupService _userCartProductItemLookupService;
        private readonly IClientDialogService _dialogService;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS
        public async Task<UserProductViewState> GetCartProductViewStateAsync(int productId) =>
           await _userProductViewStateLookupService.GetStateAsync(productId);

        public async Task<UserCartProductItemViewState> GetCartProductItemViewStateAsync(int productId) =>
            await _userCartProductItemLookupService.GetStateAsync(productId);

        public async Task AddUserCartProductAsync(int productId, int quantity = 1)
        {
            var product = await _userProductViewStateLookupService.GetStateAsync(productId);
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            //If PurchaseOptions is And then we cannot set PayType other than Mixed.
            if (product.PurchaseOptions == PurchaseOptionType.And)
            {
                productItem.PayType = OrderLinePayType.Mixed;
            }

            if (product.IsStockLimited)
            {
                //TODO: A CHECK STOCK?
            }

            try
            {
                var checkResult = await _gizmoClient.UserProductAvailabilityCheckAsync(new UserOrderLineModelCreate()
                {
                    Guid = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = productItem.Quantity + quantity,
                    PayType = productItem.PayType
                });

                if (checkResult == UserProductAvailabilityCheckResult.Success)
                {
                    productItem.Quantity += quantity;

                    await UpdateUserCartProductsAsync();

                    productItem.RaiseChanged();

                    //If current uri is not shop or product details then navigate to shop.
                    var currentUri = NavigationService.GetUri();

                    //TODO: A USE CONSTS?
                    if (!currentUri.EndsWith("/shop") && !currentUri.Contains("/productdetails"))
                        NavigationService.NavigateTo(ClientRoutes.ShopRoute);
                }
                else
                {
                    //TODO: AAA DIALOG?
                    await _dialogService.ShowAlertDialogAsync("Error", checkResult.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User product availability check error.");
            }
            finally
            {
            }
        }
        public async Task RemoveUserCartProductAsync(int productId, int quantity = 1)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity -= quantity;

            await UpdateUserCartProductsAsync();

            productItem.RaiseChanged();
        }

        public async Task DeleteUserCartProductAsync(int productId)
        {
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            productItem.Quantity = 0;

            await UpdateUserCartProductsAsync();

            productItem.RaiseChanged();
        }

        public async Task ClearUserCartProductsAsync()
        {
            var productItems = await _userCartProductItemLookupService.GetStatesAsync();
            var products = productItems.Where(x => x.Quantity > 0).ToList();

            foreach (var item in products)
            {
                item.Quantity = 0;
            }

            await UpdateUserCartProductsAsync();

            foreach (var item in products)
            {
                item.RaiseChanged();
            }
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
                //TODO: A RaiseChanged ?
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
        }

        public void SetOrderPaymentMethod(int? paymentMethodId)
        {
            ViewState.PaymentMethodId = paymentMethodId;
            ValidateProperty(() => ViewState.PaymentMethodId);
        }

        public async Task ChangeProductPayTypeAsync(int productId, OrderLinePayType payType)
        {
            var product = await _userProductViewStateLookupService.GetStateAsync(productId);
            var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

            //If PurchaseOptions is And then we cannot set PayType other than Mixed.
            if (product.PurchaseOptions == PurchaseOptionType.And)
            {
                if (productItem.PayType != OrderLinePayType.Mixed)
                {
                    productItem.PayType = OrderLinePayType.Mixed;
                    productItem.RaiseChanged();
                }
                return;
            }

            productItem.PayType = payType;

            await UpdateUserCartProductsAsync();

            productItem.RaiseChanged();
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
                var productItems = await _userCartProductItemLookupService.GetStatesAsync();
                var products = productItems.Where(x => x.Quantity > 0).ToList();

                var result = await _gizmoClient.UserOrderCreateAsync(new UserOrderModelCreate()
                {
                    UserNote = ViewState.Notes,
                    PreferredPaymentMethodId = ViewState.PaymentMethodId,
                    OrderLines = products.Select(a => new UserOrderLineModelCreate()
                    {
                        ProductId = a.ProductId,
                        Quantity = a.Quantity,
                        PayType = a.PayType
                    }).ToList()
                });

                if (result.Result == UserOrderCreateResult.Success)
                {
                    //Clear
                    await ResetAsync();
                }
                else
                {
                    await _dialogService.ShowAlertDialogAsync("Error", result.Result.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User order create error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = ex.ToString();

                await _dialogService.ShowAlertDialogAsync("Error", ex.ToString());
            }
            finally
            {
                ViewState.IsComplete = true;
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public async Task ResetAsync()
        {
            ViewState.Notes = null;
            ViewState.PaymentMethodId = null;

            await ClearUserCartProductsAsync();

            ViewState.IsComplete = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ViewState.RaiseChanged();
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
