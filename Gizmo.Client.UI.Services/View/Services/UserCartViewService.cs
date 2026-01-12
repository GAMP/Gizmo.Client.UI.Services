using System.Globalization;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Clients;
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
            IClientDialogService dialogService,
            IClientNotificationService notificationService,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            UserBalanceViewState userBalanceViewState,
            IOptions<ClientShopOptions> shopOptions,
            ClientServerCartViewService clientServerCartViewService,
            UserProductViewStateLookupService userProductViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
            _notificationService = notificationService;
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _userBalanceViewState = userBalanceViewState;
            _shopOptions = shopOptions;
            _clientServerCartViewService = clientServerCartViewService;
            _userProductViewStateLookupService = userProductViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        private readonly IClientNotificationService _notificationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly UserBalanceViewState _userBalanceViewState;
        private readonly IOptions<ClientShopOptions> _shopOptions;
        private readonly ClientServerCartViewService _clientServerCartViewService;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;

        private Guid? _lastCartId = null;
        private AddDialogResult<EmptyComponentResult>? _checkoutDialog = null;
        #endregion

        #region FUNCTIONS

        private async Task TryResetCart(CancellationToken cancellationToken = default)
        {
            try
            {
                await _clientServerCartViewService.ResetAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Cart reset error.");
            }
        }

        public async Task ClearUserCartProductsAsync()
        {
            var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_GEN_VERIFY"), _localizationService.GetString("GIZ_SHOP_VERIFY_CLEAR_CART"), AlertDialogButtons.YesNo);
            if (s.Result == AddComponentResultCode.Opened)
            {
                var result = await s.WaitForResultAsync();

                if (s.Result == AddComponentResultCode.Ok && result!.Button == AlertDialogResultButton.Yes)                
                    _clientServerCartViewService.Clear();                
            }
        }

        //private async Task UpdateUserCartProductsAsync(CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var productItems = await _userCartProductItemLookupService.GetStatesAsync(cancellationToken);

        //        ViewState.Products = productItems.Where(x => x.Quantity > 0).ToList();

        //        foreach (var item in ViewState.Products)
        //        {
        //            var product = await _userProductViewStateLookupService.GetStateAsync(item.ProductId, false, cancellationToken);

        //            item.TotalPrice = product.UnitPrice * item.Quantity;
        //            item.TotalPointsPrice = product.UnitPointsPrice * item.Quantity;
        //            item.TotalPointsAward = product.UnitPointsAward * item.Quantity;
        //            //TODO: A RaiseChanged ?
        //        }

        //        ViewState.Total = ViewState.Products.Where(a => a.PayType == OrderLinePayType.Cash || a.PayType == OrderLinePayType.Mixed).Select(a => a.TotalPrice).Sum();
        //        ViewState.PointsTotal = ViewState.Products.Where(a => a.PayType == OrderLinePayType.Points || a.PayType == OrderLinePayType.Mixed).Select(a => (a.TotalPointsPrice ?? 0)).Sum();
        //        ViewState.PointsAward = ViewState.Products.Select(a => (a.TotalPointsAward ?? 0)).Sum();

        //        if (ViewState.Total == 0)
        //        {
        //            //Payment method is not required, it's not even visible when the total price is 0.
        //            //In case the user previously had validation error for payment method, the submit is blocked.
        //            //Clear this error to unblock order submit.
        //            ClearError(() => ViewState.PaymentMethodId);
        //        }

        //        ViewState.RaiseChanged();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, "Failed to update user cart products.");
        //    }
        //}

        public void SetNotes(string value)
        {
            ViewState.Notes = value;
            ValidateProperty(() => ViewState.Notes);
        }

        public void SetOrderPaymentMethod(int? paymentMethodId)
        {
            ViewState.PaymentMethodId = paymentMethodId;
            ValidateProperty(() => ViewState.PaymentMethodId);

            if (paymentMethodId.HasValue)
                _clientServerCartViewService.AddPayment(paymentMethodId.Value, 0);
            else
                _clientServerCartViewService.RemovePayment(0);
        }

        //public async Task ChangeProductPayTypeAsync(int productId, OrderLinePayType payType)
        //{
        //    try
        //    {
        //        var productItem = await _userCartProductItemLookupService.GetStateAsync(productId);
        //        if (productItem.PayType == payType)
        //            return;

        //        if ((payType == OrderLinePayType.Points || productItem.PayType == OrderLinePayType.Mixed) && product.UnitPointsPrice > 0)
        //        {
        //            if (ViewState.PointsTotal + (product.UnitPointsPrice * productItem.Quantity) > _userBalanceViewState.PointsBalance)
        //            {
        //                await UpdateUserCartProductsAsync();
        //                productItem.RaiseChanged();

        //                await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_GEN_ERROR"), _localizationService.GetString("GIZ_INSUFFICIENT_POINTS_ERROR_MESSAGE"), AlertDialogButtons.OK, AlertTypes.Danger);

        //                return;
        //            }
        //        }

        //        productItem.PayType = payType;

        //        await UpdateUserCartProductsAsync();
        //        productItem.RaiseChanged();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogError(ex, "Failed to change user cart product pay type.");
        //    }
        //}
        
        public async Task RemovePromocodeAsync()
        {
            _clientServerCartViewService.RemovePromoCode();
        }

        public async Task SubmitAsync()
        {
            if (_clientServerCartViewService.ViewState.Total == 0)
            {
                ViewState.ShowPaymentMethods = false;
            }
            else
            {
                ViewState.ShowPaymentMethods = true;
            }
            
            ClearDialog();

            _checkoutDialog = await _dialogService.ShowCheckoutDialogAsync();
            if (_checkoutDialog.Result == AddComponentResultCode.Opened)
                await _checkoutDialog.WaitForResultAsync();

            _checkoutDialog = null;
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
                await _clientServerCartViewService.AcceptAsync(ViewState.Notes);

                ViewState.HasError = false;
                ViewState.ErrorMessage = string.Empty;

                //TODO: AAAAA CHECK
                //        if (result.Result == OrderResult.Failed)
                //        {
                //            ViewState.HasError = true;
                //            ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED") + $" {result.FailReason.ToString()}"; //TODO: AAA TRANSLATE?

                //            if (result.OrderLines != null)
                //            {
                //                foreach (var orderLine in result.OrderLines)
                //                {
                //                    var requestOrderLine = userOrderModelCreate.OrderLines.Where(a => a.Guid == orderLine.Guid).FirstOrDefault();
                //                    if (requestOrderLine != null)
                //                    {
                //                        var product = await _userProductViewStateLookupService.GetStateAsync(requestOrderLine.ProductId);

                //                        string ERROR_MESSAGE = string.Empty;

                //                        switch (orderLine.Result)
                //                        {
                //                            case UserProductAvailabilityCheckResult.ClientOrderDisallowed:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_CLIENT_ORDER_DISALLOWED_MESSAGE");
                //                                break;
                //                            case UserProductAvailabilityCheckResult.UserGroupDisallowed:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_DISALLOWED_USER_GROUP_MESSAGE");
                //                                break;
                //                            case UserProductAvailabilityCheckResult.SaleDisallowed:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_SALE_DISALLOWED_MESSAGE");
                //                                break;
                //                            case UserProductAvailabilityCheckResult.GuestSaleDisallowed:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_GUEST_SALE_DISALLOWED_MESSAGE");
                //                                break;
                //                            case UserProductAvailabilityCheckResult.OutOfStock:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_OUT_OF_STOCK_MESSAGE");
                //                                break;
                //                            case UserProductAvailabilityCheckResult.PeriodDisallowed:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_PURCHASE_PERIOD_DISALLOWED_MESSAGE");
                //                                break;
                //                            //TODO: AAA DIALOG TRANSLATE MORE RESULTS
                //                            default:
                //                                ERROR_MESSAGE = _localizationService.GetString("GIZ_PRODUCT_ORDER_PASS_RESULT_ERROR_MESSAGE");
                //                                break;
                //                        }

                //                        ViewState.ErrorMessage += $"<br>{product.Name}: {ERROR_MESSAGE}";
                //                    }
                //                }
                //            }
                //        }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User order create error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
            }
            finally
            {
                //Clear
                await TryResetCart();

                ViewState.IsComplete = true;
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public void ClearCart()
        {
            ViewState.Notes = null;
            ViewState.PaymentMethodId = null;

            ViewState.RaiseChanged();
        }

        public void ClearDialog()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.IsComplete = false;

            ViewState.RaiseChanged();
        }

        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnUserLoginStateChange;
            _clientServerCartViewService.OnReset += ClientServerCartViewService_OnReset;
            return base.OnInitializing(ct);
        }

        private void ClientServerCartViewService_OnReset(object? sender, EventArgs e)
        {
            ClearCart();

            //Close dialog.
            _checkoutDialog?.Controller?.Result(new EmptyComponentResult());
        }

        private async void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.LoggingOut:

                    await TryResetCart();
                    ClearCart();

                    break;

                default:
                    break;
            }
        }

        protected override void OnDisposing(bool dis)
        {
            _clientServerCartViewService.OnReset -= ClientServerCartViewService_OnReset;
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
                    _clientServerCartViewService.AddProduct(productId, quantity); //From Adds.
                    NavigationService.NavigateTo(ClientRoutes.ShopRoute);
                    break;
            }
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.PaymentMethodId))
            {
                if (_clientServerCartViewService.ViewState.Total > 0)
                {
                    if (!ViewState.PaymentMethodId.HasValue)
                    {
                        AddError(() => ViewState.PaymentMethodId, _localizationService.GetString("GIZ_GEN_VE_REQUIRED_NAMED_FIELD", nameof(ViewState.PaymentMethodId)));
                    }
                    else if (ViewState.PaymentMethodId.Value == -3 && _clientServerCartViewService.ViewState.Total > _userBalanceViewState.Balance)
                    {
                        AddError(() => ViewState.PaymentMethodId, _localizationService.GetString("GIZ_INSUFFICIENT_DEPOSITS_MESSAGE", nameof(ViewState.PaymentMethodId)));
                    }
                }
            }
        }
    }
}
