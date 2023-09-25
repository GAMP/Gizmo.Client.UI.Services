using System.Globalization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            IClientNotificationService notificationService,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            UserBalanceViewState userBalanceViewState,
            IOptions<ClientShopOptions> shopOptions) : base(viewState, logger, serviceProvider)
        {
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _userCartProductItemLookupService = userCartProductItemLookupService;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _userBalanceViewState = userBalanceViewState;
            _shopOptions = shopOptions;
        }
        #endregion

        #region FIELDS
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly UserCartProductItemViewStateLookupService _userCartProductItemLookupService;
        private readonly IClientDialogService _dialogService;
        private readonly IClientNotificationService _notificationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly UserBalanceViewState _userBalanceViewState;
        private readonly IOptions<ClientShopOptions> _shopOptions;
        #endregion

        #region FUNCTIONS
        public async Task<UserProductViewState> GetCartProductViewStateAsync(int productId) =>
           await _userProductViewStateLookupService.GetStateAsync(productId);

        public async Task<UserCartProductItemViewState> GetCartProductItemViewStateAsync(int productId) =>
            await _userCartProductItemLookupService.GetStateAsync(productId);

        public async Task AddUserCartProductAsync(int productId, int quantity = 1)
        {
            try
            {
                var product = await _userProductViewStateLookupService.GetStateAsync(productId);
                var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

                if (product.ProductType == ProductType.ProductTime && productItem.Quantity >= 1)
                {
                    return;
                }

                //If PurchaseOptions is And then we cannot set PayType other than Mixed.
                if (product.PurchaseOptions == PurchaseOptionType.And)
                {
                    productItem.PayType = OrderLinePayType.Mixed;
                }

                if ((productItem.PayType == OrderLinePayType.Points || productItem.PayType == OrderLinePayType.Mixed) && product.UnitPointsPrice > 0)
                {
                    if (ViewState.PointsTotal + product.UnitPointsPrice > _userBalanceViewState.PointsBalance)
                    {
                        if (productItem.Quantity == 0 && product.PurchaseOptions == PurchaseOptionType.Or)
                        {
                            //There is a case where the user previously used all his points and bought this product with points.
                            //When this product is added again in the cart the pay type is Points, but the user does'n have enough points to add it in the cart.
                            //Change the pay type to Cash to unblock add to cart.
                            productItem.PayType = OrderLinePayType.Cash;
                        }
                        else
                        {
                            await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_INSUFFICIENT_POINTS_ERROR_TITLE"), _localizationService.GetString("GIZ_INSUFFICIENT_POINTS_ERROR_MESSAGE"));
                            return;
                        }
                    }
                }

                if (product.IsStockLimited ||
                    product.PurchaseAvailability != null)
                {
                    try
                    {
                        var checkResult = await _gizmoClient.UserProductAvailabilityCheckAsync(new UserOrderLineModelCreate()
                        {
                            Guid = Guid.NewGuid(),
                            ProductId = productId,
                            Quantity = productItem.Quantity + quantity,
                            PayType = productItem.PayType
                        });

                        if (checkResult != UserProductAvailabilityCheckResult.Success)
                        {
                            string ERROR_MESSAGE = string.Empty;

                            switch (checkResult)
                            {
                                case UserProductAvailabilityCheckResult.ClientOrderDisallowed:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_CLIENT_ORDER_DISALLOWED_MESSAGE");
                                    break;
                                case UserProductAvailabilityCheckResult.UserGroupDisallowed:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_DISALLOWED_USER_GROUP_MESSAGE");
                                    break;
                                case UserProductAvailabilityCheckResult.SaleDisallowed:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_SALE_DISALLOWED_MESSAGE");
                                    break;
                                case UserProductAvailabilityCheckResult.GuestSaleDisallowed:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_GUEST_SALE_DISALLOWED_MESSAGE");
                                    break;
                                case UserProductAvailabilityCheckResult.OutOfStock:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_OUT_OF_STOCK_MESSAGE");
                                    break;
                                case UserProductAvailabilityCheckResult.PeriodDisallowed:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_PURCHASE_PERIOD_DISALLOWED_MESSAGE");
                                    break;
                                //TODO: AAA DIALOG TRANSLATE MORE RESULTS
                                default:
                                    ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_ERROR_MESSAGE");
                                    break;
                            }

                            await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_ERROR_TITLE"), ERROR_MESSAGE);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "User product availability check error.");
                        return;
                    }
                    finally
                    {
                    }
                }

                if (productItem.Quantity == 0 && product.ProductType == ProductType.ProductTime && product.TimeProduct?.UsageAvailability != null)
                {
                    bool verifyNotAvailableTimeProduct = false;

                    if (product.TimeProduct.UsageAvailability.DateRange)
                    {
                        if ((product.TimeProduct.UsageAvailability.StartDate.HasValue && product.TimeProduct.UsageAvailability.StartDate.Value > DateTime.Now) ||
                            (product.TimeProduct.UsageAvailability.EndDate.HasValue && product.TimeProduct.UsageAvailability.EndDate.Value < DateTime.Now))
                        {
                            verifyNotAvailableTimeProduct = true;
                        }
                    }

                    if (product.TimeProduct.UsageAvailability.TimeRange)
                    {
                        var daySecond = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).TotalSeconds;

                        if (product.TimeProduct.UsageAvailability.DaysAvailable.Where(day => day.Day == DateTime.Now.DayOfWeek && day.DayTimesAvailable != null && day.DayTimesAvailable.Where(time => time.StartSecond <= daySecond && time.EndSecond > daySecond).Any()).Any() == false)
                        {
                            verifyNotAvailableTimeProduct = true;
                        }
                    }

                    if (verifyNotAvailableTimeProduct)
                    {
                        var dialogResult = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_GEN_WARNING"), _localizationService.GetString("GIZ_PRODUCT_TIME_CURRENTLY_UNAVAILABLE_VERIFY"), AlertDialogButtons.YesNo, AlertTypes.Warning);
                        var dialogResponse = await dialogResult.WaitForResultAsync();
                        if (dialogResponse?.Button == AlertDialogResultButton.No)
                        {
                            return;
                        }
                    }
                }

                productItem.Quantity += quantity;

                await UpdateUserCartProductsAsync();

                productItem.RaiseChanged();

                //If current uri is not shop or product details then navigate to shop.
                var currentUri = NavigationService.GetUri();

                //TODO: A USE CONSTS?
                if (!currentUri.EndsWith("/shop") && !currentUri.Contains("/productdetails"))
                    NavigationService.NavigateTo(ClientRoutes.ShopRoute);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to add user cart product.");
            }
        }

        public async Task RemoveUserCartProductAsync(int productId, int quantity = 1)
        {
            try
            {
                var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

                productItem.Quantity -= quantity;

                await UpdateUserCartProductsAsync();

                productItem.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to remove user cart product.");
            }
        }

        public async Task DeleteUserCartProductAsync(int productId)
        {
            try
            {
                var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);

                productItem.Quantity = 0;

                await UpdateUserCartProductsAsync();

                productItem.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to delete user cart product.");
            }
        }

        private async Task ClearProductsAsync()
        {
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to clear user cart products.");
            }
        }

        public async Task ClearUserCartProductsAsync()
        {
            var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_GEN_VERIFY"), _localizationService.GetString("GIZ_SHOP_VERIFY_CLEAR_CART"), AlertDialogButtons.YesNo);
            if (s.Result == AddComponentResultCode.Opened)
            {
                var result = await s.WaitForResultAsync();

                if (s.Result == AddComponentResultCode.Ok && result!.Button == AlertDialogResultButton.Yes)
                    await ClearProductsAsync();
            }
        }

        private async Task UpdateUserCartProductsAsync(CancellationToken cancellationToken = default)
        {
            try
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

                if (ViewState.Total == 0)
                {
                    //Payment method is not required, it's not even visible when the total price is 0.
                    //In case the user previously had validation error for payment method, the submit is blocked.
                    //Clear this error to unblock order submit.
                    ClearError(() => ViewState.PaymentMethodId);
                }

                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update user cart products.");
            }
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
            try
            {
                var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);
                if (productItem.PayType == payType)
                    return;

                var product = await _userProductViewStateLookupService.GetStateAsync(productId);

                //If PurchaseOptions is And then we cannot set PayType other than Mixed.
                if (product.PurchaseOptions == PurchaseOptionType.And)
                {
                    if (productItem.PayType != OrderLinePayType.Mixed)
                    {
                        productItem.PayType = OrderLinePayType.Mixed;
                    }

                    await UpdateUserCartProductsAsync();
                    productItem.RaiseChanged();

                    return;
                }

                if ((payType == OrderLinePayType.Points || productItem.PayType == OrderLinePayType.Mixed) && product.UnitPointsPrice > 0)
                {
                    if (ViewState.PointsTotal + (product.UnitPointsPrice * productItem.Quantity) > _userBalanceViewState.PointsBalance)
                    {
                        await UpdateUserCartProductsAsync();
                        productItem.RaiseChanged();

                        await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_GEN_ERROR"), _localizationService.GetString("GIZ_INSUFFICIENT_POINTS_ERROR_MESSAGE"), AlertDialogButtons.OK, AlertTypes.Danger);

                        return;
                    }
                }

                productItem.PayType = payType;

                await UpdateUserCartProductsAsync();
                productItem.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to change user cart product pay type.");
            }
        }

        public async Task SubmitAsync()
        {
            if (ViewState.Total == 0)
            {
                ViewState.ShowPaymentMethods = false;
            }
            else
            {
                ViewState.ShowPaymentMethods = true;
            }

            var s = await _dialogService.ShowCheckoutDialogAsync();
            if (s.Result == AddComponentResultCode.Opened)
                _ = await s.WaitForResultAsync();
        }

        public async Task CheckoutAsync()
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

                var userOrderModelCreate = new UserOrderModelCreate()
                {
                    UserNote = ViewState.Notes,
                    PreferredPaymentMethodId = ViewState.PaymentMethodId,
                    OrderLines = products.Select(a => new UserOrderLineModelCreate()
                    {
                        Guid = Guid.NewGuid(),
                        ProductId = a.ProductId,
                        Quantity = a.Quantity,
                        PayType = a.PayType,
                        Total = (a.PayType == OrderLinePayType.Cash || a.PayType == OrderLinePayType.Mixed) ? a.TotalPrice : 0,
                        PointsTotal = (a.PayType == OrderLinePayType.Points || a.PayType == OrderLinePayType.Mixed) ? a.TotalPointsPrice.GetValueOrDefault() : 0,
                        PointsAwardTotal = a.PayType != OrderLinePayType.Points ? a.TotalPointsAward.GetValueOrDefault() : 0
                    }).ToList()
                };

                var result = await _gizmoClient.UserOrderCreateAsync(userOrderModelCreate);

                if (result.Result != OrderResult.Failed)
                {
                    //Clear
                    await ResetAsync();
                }
                else
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED") + $" {result.FailReason.ToString()}"; //TODO: AAA TRANSLATE?

                    if (result.OrderLines != null)
                    {
                        foreach (var orderLine in result.OrderLines)
                        {
                            var requestOrderLine = userOrderModelCreate.OrderLines.Where(a => a.Guid == orderLine.Guid).FirstOrDefault();
                            if (requestOrderLine != null)
                            {
                                var product = await _userProductViewStateLookupService.GetStateAsync(requestOrderLine.ProductId);

                                string ERROR_MESSAGE = string.Empty;

                                switch (orderLine.Result)
                                {
                                    case UserProductAvailabilityCheckResult.ClientOrderDisallowed:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_CLIENT_ORDER_DISALLOWED_MESSAGE");
                                        break;
                                    case UserProductAvailabilityCheckResult.UserGroupDisallowed:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_DISALLOWED_USER_GROUP_MESSAGE");
                                        break;
                                    case UserProductAvailabilityCheckResult.SaleDisallowed:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_SALE_DISALLOWED_MESSAGE");
                                        break;
                                    case UserProductAvailabilityCheckResult.GuestSaleDisallowed:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_GUEST_SALE_DISALLOWED_MESSAGE");
                                        break;
                                    case UserProductAvailabilityCheckResult.OutOfStock:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_OUT_OF_STOCK_MESSAGE");
                                        break;
                                    case UserProductAvailabilityCheckResult.PeriodDisallowed:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_PURCHASE_PERIOD_DISALLOWED_MESSAGE");
                                        break;
                                    //TODO: AAA DIALOG TRANSLATE MORE RESULTS
                                    default:
                                        ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_ERROR_MESSAGE");
                                        break;
                                }

                                ViewState.ErrorMessage += $"<br>{product.Name}: {ERROR_MESSAGE}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User order create error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
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
            try
            {
                ViewState.Notes = null;
                ViewState.PaymentMethodId = null;

                await ClearProductsAsync();

                ViewState.IsComplete = false;
                ViewState.HasError = false;
                ViewState.ErrorMessage = string.Empty;

                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to reset user cart products.");
            }
        }

        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnUserLoginStateChange;
            return base.OnInitializing(ct);
        }

        private async void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            try
            {
                switch (e.State)
                {
                    case LoginState.LoggingOut:
                        await ClearProductsAsync();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling login state change.");
            }
        }

        protected override void OnDisposing(bool dis)
        {
            _gizmoClient.LoginStateChange -= OnUserLoginStateChange;
            base.OnDisposing(dis);
        }

        public override bool ValidateCommand<TCommand>(TCommand command)
        {
            if (_shopOptions.Value.Disabled)
                return false;

            if (command.Type != ViewServiceCommandType.Add)
                return false;

            if (command.Params?.Any() != true)
                return false;

            var paramProductId = command.Params.GetValueOrDefault("productId")?.ToString();

            if (paramProductId is null)
                return false;

            return true;
        }

        public override async Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default)
        {
            if (_shopOptions.Value.Disabled)
                return;

            if (command.Params?.Any() != true)
                return;

            var paramProductId = command.Params.GetValueOrDefault("productId")?.ToString();

            if (paramProductId is null)
                return;

            var productId = int.Parse(paramProductId, NumberStyles.Number);

            var products = await _userProductViewStateLookupService.GetStatesAsync();
            if (!products.Where(a => a.Id == productId).Any())
            {
                NavigationService.NavigateTo(ClientRoutes.NotFoundRoute);
                return;
            }

            var paramQuantity = command.Params.GetValueOrDefault("quantity")?.ToString();

            var quantity = 1;
            if (paramQuantity is not null)
                quantity = int.Parse(paramQuantity, NumberStyles.Number);

            switch (command.Type)
            {
                case ViewServiceCommandType.Add:
                    await AddUserCartProductAsync(productId, quantity);
                    NavigationService.NavigateTo(ClientRoutes.ShopRoute);
                    break;
            }
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

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.PaymentMethodId))
            {
                if (ViewState.Total > 0)
                {
                    if (!ViewState.PaymentMethodId.HasValue)
                    {
                        AddError(() => ViewState.PaymentMethodId, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_NAMED_FIELD", nameof(ViewState.PaymentMethodId)));
                    }
                    else if (ViewState.PaymentMethodId.Value == -3 && ViewState.Total > _userBalanceViewState.Balance)
                    {
                        AddError(() => ViewState.PaymentMethodId, _localizationService.GetString("GIZ_INSUFFICIENT_DEPOSITS_MESSAGE", nameof(ViewState.PaymentMethodId)));
                    }
                }
            }
        }
    }
}
