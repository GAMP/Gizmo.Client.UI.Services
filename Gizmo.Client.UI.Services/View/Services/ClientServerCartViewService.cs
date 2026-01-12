using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.Server.Exceptions;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ClientServerCartViewService : CartViewServiceBase<ClientServerCartViewState>
    {
        #region CONSTRUCTOR
        public ClientServerCartViewService(
            ClientServerCartViewState viewState,
            GlobalCancellationService globalCancellationService,
            ILocalizationService localizationService,
            ILogger<UserCartViewService> logger,
            IServiceProvider serviceProvider,
            UserProductViewStateLookupService userProductViewStateLookupService,
            Web.Api.User.Clients.CartsWebApiClient cartsWebApiClient,
            IClientNotificationService notificationService,
            IClientDialogService dialogService,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService) : base(viewState, globalCancellationService, localizationService, logger, serviceProvider)
        {
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _cartsWebApiClient = cartsWebApiClient;
            _notificationService = notificationService;
            _dialogService = dialogService;
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
        }
        #endregion

        public event EventHandler? OnReset;

        #region FIELDS

        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly Web.Api.User.Clients.CartsWebApiClient _cartsWebApiClient;
        private readonly IClientNotificationService _notificationService;
        private readonly IClientDialogService _dialogService;
        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;

        #endregion

        public Task<UserCartProductViewState?> GetUserCartProductViewStateAsync(int productId)
        {
            return Task.FromResult<UserCartProductViewState?>(ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault());
        }

        public async Task<bool> SetNote(string? note, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                await _cartsWebApiClient.NoteAsync(currentCartId, note, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), ex.Message);

                return false;
            }
        }
        
        public async Task ResetAsync(CancellationToken cancellationToken = default)
        {
            await _cartCreateLock.WaitAsync(cancellationToken);
            try
            {
                try
                {
                    // check if we created cart already
                    if (_currentCartId.HasValue)
                        await _cartsWebApiClient.DeleteAsync(_currentCartId.Value, cancellationToken);
                }
                catch (Exception ex)
                {
                    // only report non invalid cart errors, otherwise forward to lower handler
                    if (!ex.IsExceptionCode(ExceptionCode.Cart, CartErrorCode.InvalidCartId))
                        throw;
                }

                ViewState.Clear();
                DebounceViewStateChanged();

                OnReset?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), ex.Message);
            }
            finally
            {
                _currentCartId = null;
                _cartCreateLock.Release();
            }
        }

        private async ValueTask InvalidateCartAsync(WebApiClientException webApiException, CancellationToken cancellationToken = default)
        {
            // here we need to check the api error code, an potential problem here is when the cart expires on server
            // in such case the local state have no meaning and we should inform the user and reset the local cart state

            if (webApiException.ErrorCodeType == (int?)ExceptionCode.Cart && webApiException.ErrorCode == (int)CartErrorCode.InvalidCartId)
            {
                // reset state here               
                try
                {
                    await _cartCreateLock.WaitAsync(cancellationToken);
                    _currentCartId = null;
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex, "Unexpected error on cart invalidation.");
                }
                finally
                {
                    _cartCreateLock.Release();
                }

                // report invalid cart error to the user
                //await _errorHandlerService.Handle(ExceptionErrorContext.User(webApiException), nameof(Gizmo.Web.Manager.UI.Resources.Autogenerated.Resources.WEBM_GEN_ERROR_TITLE), null, null, cancellationToken);
                await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), webApiException.Message);

                // clear cart
                ViewState.Clear();
                DebounceViewStateChanged();

                OnReset?.Invoke(this, EventArgs.Empty);
            }
        }

        protected void ThrowInfInvalidCartError(WebApiClientException webApiException)
        {
            if (webApiException.IsExceptionCode(ExceptionCode.Cart, CartErrorCode.InvalidCartId))
                throw webApiException;
        }

        public async Task AcceptAsync(string? note, CancellationToken cancellationToken = default)
        {
            //try
            //{
            var currentCartId = await CartGetOrCreateAsync(cancellationToken);
            await _cartsWebApiClient.AcceptAsync(currentCartId, new UserCartAcceptModel()
            {
                Note = note
            }, cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //    await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), ex.Message);
            //}
        }

        public override async Task AcceptAsync(CancellationToken cancellationToken = default)
        {
            //try
            //{
            var currentCartId = await CartGetOrCreateAsync(cancellationToken);
            await _cartsWebApiClient.AcceptAsync(currentCartId, new UserCartAcceptModel()
            {
            }, cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //    await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), ex.Message);
            //}
        }

        protected override async Task<bool> ValidateRequestAsync(ICartRequest request, CancellationToken cancellationToken = default)
        {
            // this function serves as pre-validation for the cart requests so we don't try to send them server side
            // the goal is to filter out buggy requests and log them nothing more         

            if (request is SetQuantityRequest setQuantityRequest)
            {
                if (setQuantityRequest.Quantity <= 0)
                {
                    // negative or zero quantity not allowed, use Remove instead
                    return false;
                }
            }
            else if (request is AddProductRequest addProductRequest)
            {
                // check the product being added
                var productViewState = await _userProductViewStateLookupService.GetStateAsync(addProductRequest.ProductId, cancellationToken);
                if (productViewState.ProductType == ProductType.ProductTime)
                {
                    if (productViewState.ProductType == ProductType.ProductTime && productViewState.TimeProduct?.UsageAvailability != null)
                    {
                        bool verifyNotAvailableTimeProduct = false;

                        if (productViewState.TimeProduct.UsageAvailability.DateRange)
                        {
                            if ((productViewState.TimeProduct.UsageAvailability.StartDate.HasValue && productViewState.TimeProduct.UsageAvailability.StartDate.Value > DateTime.Now) ||
                                (productViewState.TimeProduct.UsageAvailability.EndDate.HasValue && productViewState.TimeProduct.UsageAvailability.EndDate.Value < DateTime.Now))
                            {
                                verifyNotAvailableTimeProduct = true;
                            }
                        }

                        if (productViewState.TimeProduct.UsageAvailability.TimeRange)
                        {
                            var daySecond = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).TotalSeconds;

                            if (productViewState.TimeProduct.UsageAvailability.DaysAvailable.Where(day => day.Day == DateTime.Now.DayOfWeek && day.DayTimesAvailable != null && day.DayTimesAvailable.Where(time => time.StartSecond <= daySecond && time.EndSecond > daySecond).Any()).Any() == false)
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
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        protected override async Task<ICartRequest> PreProcessRequestAsync(ICartRequest request, CancellationToken cancellationToken)
        {
            if (request is SetQuantityRequest setQuantityRequest)
            {
                if (ViewState.TryGetProductEntryViewState(setQuantityRequest.EntryId, out var productEntryViewState))
                {
                    productEntryViewState.Quantity = (int)setQuantityRequest.Quantity;
                    productEntryViewState.RaiseChanged();
                }
            }
            else if (request is SetPayTypeRequest setPayTypeRequest)
            {
                if (ViewState.TryGetProductEntryViewState(setPayTypeRequest.EntryId, out var productEntryViewState))
                {
                    productEntryViewState.PayType = setPayTypeRequest.PayType;
                    productEntryViewState.RaiseChanged();
                }
            }
            else if (request is RemoveEntryRequest removeEntryRequest)
            {
                // we can remove entry right away, what will happen is that if once we try to remove entry from server cart and fail
                // the entry will reappear after an server cart refresh, we will be able to notify user of an error then
                ViewState.TryRemoveEntry(removeEntryRequest.EntryId);
                ViewState.RaiseChanged();
            }
            else if (request is ClearCartRequest)
            {
                ViewState.Clear();
                ViewState.RaiseChanged();
            }
            else if (request is AddPomoCodeRequest || request is RemovePromoCodeRequest)
            {
                ViewState.PromoCodeViewState.IsLoading = true;
                ViewState.PromoCodeViewState.RaiseChanged();
            }

            return await ValueTask.FromResult(request);
        }

        protected override async ValueTask HandleRequestErrorAsync(Exception exception, ICartRequest request, CartRequestContext cartRequestContext, CancellationToken cancellationToken = default)
        {
            // the handler should always throw once invalid cart error occurs

            if (exception is WebApiClientException webApiException)
            {
                ThrowInfInvalidCartError(webApiException);

                // the error is not invalid cart id here

                // handle know/expected errors
                if (webApiException.ErrorCodeType == (int)ExceptionCode.Promotion || webApiException.ErrorCodeType == (int)ExceptionCode.Cart)
                {
                    //await _errorHandlerService.Handle(ExceptionErrorContext.User(webApiException), nameof(Gizmo.Web.Manager.UI.Resources.Autogenerated.Resources.WEBM_GEN_ERROR_TITLE), null, null, cancellationToken);
                    await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), webApiException.Message);
                }
            }
        }

        protected override async ValueTask HandleProcessingErrorAsync(Exception exception, CancellationToken cancellationToken = default)
        {
            if (exception is WebApiClientException webApiClientException)
            {
                await InvalidateCartAsync(webApiClientException, cancellationToken);
            }
        }

        protected override async Task RefreshStateAsync(CancellationToken cancellationToken = default)
        {
            // it should be clear here that the server side cart is the ultimate source of truth and we should expect that
            // quantities on lines can change, lines can be added and removed e.t.c 
            // we always need to bring the local cart state to match server side

            ViewState.IsStateUpdating = true;
            DebounceViewStateChanged();

            try
            {
                // get current cart id, its ok to create a new one, since we calling this procedure some lines should have been added already so the cart should be created at that time
                var cartId = await CartGetOrCreateAsync(cancellationToken);

                // obtain current cart state
                var cartStateModel = await _cartsWebApiClient.StateAsync(cartId, cancellationToken);

                // create list of user cart ids on the server
                var serverSideUsers = cartStateModel.Carts.Select(userCartStateModel => userCartStateModel.UserId).ToArray();

                var userCartStateModel = cartStateModel.Carts.FirstOrDefault();
                if (userCartStateModel != null)
                {
                    foreach (var cartEntryModel in userCartStateModel.Entries)
                    {
                        if (cartEntryModel is CartEntryProductModel productModel)
                        {
                            if (!ViewState.TryGetProductEntryViewState(productModel.Id, out var entryViewState) || entryViewState == null)
                            {
                                var product = await _userProductViewStateLookupService.GetStateAsync(productModel.ProductId, cancellationToken);

                                entryViewState = ViewState.Add(new UserCartProductViewState()
                                {
                                    ProductId = productModel.ProductId,
                                    Guid = productModel.Id,
                                    PurchaseOptions = product.PurchaseOptions,
                                    ProductEntityType = product.ProductType,
                                    ProductName = product.Name,
                                });
                            }

                            entryViewState.UnitPrice = productModel.UnitPrice;
                            entryViewState.UnitPointsPrice = productModel.UnitPointsPrice;
                            entryViewState.Quantity = (int)productModel.Quantity;
                            entryViewState.TotalPointsAward = productModel.PointsAward;
                            entryViewState.TotalPrice = productModel.Total;
                            entryViewState.TotalPointsPrice = productModel.PointsTotal;
                            entryViewState.OriginalUnitPrice = productModel.UnitListPrice;
                            entryViewState.IsCustomPrice = productModel.UnitPrice != productModel.UnitListPrice;
                        }
                        else
                        {
                            // unexpected entry ?
                            Logger.LogWarning("Unexpected cart entry {entryType} encountered.", cartEntryModel.GetType());
                            continue;
                        }
                    }

                    // since the server cart can dynamically change we need to sync all the local entries
                    // potentially some entries will be removed and some might get added, Guid should server as virtualization key for optimal re-rendering (if we will use virtualization)
                    var currentEntries = userCartStateModel.Entries;

                    // update user cart values

                    ViewState.SubTotal = userCartStateModel.SubTotal;
                    ViewState.TaxTotal = userCartStateModel.TaxTotal;
                    ViewState.Total = userCartStateModel.Total;
                    ViewState.PointsTotal = userCartStateModel.PointsTotal;

                    // promotion code status
                    ViewState.PromoCodeStatus = userCartStateModel.PromoCodeStatus;

                    // TODO : debounce at the end
                    ViewState.RaiseChanged();
                }

                if (cartStateModel.PromotionState != null)
                {
                    // TODO : implement caching or an custom api for better performance
                    // we also don't need to pull this information on each refresh, just updating it on initial addition of promocode is required

                    var promoCodeStateModel = cartStateModel.PromotionState;
                    var promoCodeViewState = ViewState.PromoCodeViewState;

                    if (promoCodeViewState.PromoCodeId == null || promoCodeViewState.PromoCodeId != promoCodeStateModel.PromoCodeId)
                    {
                        int promoCodeId = promoCodeStateModel.PromoCodeId;
                        //TODO: AAAAA REVIEW
                        //var promoCode = await _promotionsWebApiClient.PromotionCodeAsync(promoCodeId, cancellationToken);
                        //var promotion = await _promotionsWebApiClient.GetByIdAsync(promoCode.PromotionId, cancellationToken);

                        //string name = (promotion as PromotionDiscountModel)?.Name ?? (promotion as PromotionDiscountModel)?.Name ?? string.Empty;
                        //string description = (promotion as PromotionDiscountModel)?.Description ?? (promotion as PromotionDiscountModel)?.Description ?? string.Empty;

                        //IList<string> discountNames = [];
                        //if (promotion is PromotionDiscountModel promotionDiscount)
                        //{
                        //    var discount = await _discountsWebApiClient.GetByIdAsync(promotionDiscount.DiscountId, cancellationToken);
                        //    discountNames.Add(discount.Name);
                        //}
                        //else if (promotion is PromotionDiscountGroupModel promotionDiscountGroup)
                        //{
                        //    var discountGroup = await _discountGroupsWebApiClient.GetByIdAsync(promotionDiscountGroup.DiscountGroupId, cancellationToken);
                        //    foreach (var discountGroupDiscount in discountGroup.Discounts)
                        //    {
                        //        var discount = await _discountsWebApiClient.GetByIdAsync(discountGroupDiscount.DiscountId, cancellationToken);
                        //        discountNames.Add(discount.Name);
                        //    }
                        //}

                        promoCodeViewState.PromoCodeId = promoCodeId;

                        //// TODO : only update value if none set, this will allow us to keep the promo code value same way user have typed it, not sure if it is desired based on UI designs and promo code rules
                        //if (string.IsNullOrWhiteSpace(promoCodeViewState.PromoCode))
                        //    promoCodeViewState.PromoCode = promoCode.Value;

                        //promoCodeViewState.DiscountNames = discountNames;
                        //promoCodeViewState.Name = name;
                        //promoCodeViewState.Description = description;
                        promoCodeViewState.RaiseChanged();
                    }
                }
                else
                {
                    // TODO : we would only need to call this if we have applied promo code previously
                    ViewState.ResetPromotionState();
                }

                // TODO : process payments 


                // update local cart view state with new values
                ViewState.PointsTotal = cartStateModel.PointsTotal;
                ViewState.SubTotal = cartStateModel.SubTotal;
                ViewState.TaxTotal = cartStateModel.TaxTotal;
                ViewState.Discount = cartStateModel.Discount;
                ViewState.FeeTotal = cartStateModel.FeeTotal;
                ViewState.Total = cartStateModel.Total;
                ViewState.PointsAward = cartStateModel.PointsAward;
            }
            catch (Exception ex)
            {
                //await _errorHandlerService.Handle(ExceptionErrorContext.Service(ex), new ErrorModel(), cancellationToken);
                await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), ex.Message);

                //TODO: AAAAA reset here?
            }
            finally
            {
                ViewState.IsStateUpdating = false;
                DebounceViewStateChanged();
            }
        }

        protected override async Task<Guid> CartGetOrCreateAsync(CancellationToken cancellationToken = default)
        {
            // obtain cart lock
            await _cartCreateLock.WaitAsync(cancellationToken);
            try
            {
                // check if cart is already created
                if (_currentCartId.HasValue)
                    return _currentCartId.Value;

                // create cart on server
                var cartCreateResult = await _cartsWebApiClient.CreateAsync(cancellationToken);

                _currentCartId = cartCreateResult.Id;

                return _currentCartId.Value;
            }
            catch (OperationCanceledException)
            {
                // operation was cancelled, we still need to forward the cancellation exception to caller

                throw;
            }
            catch
            {
                // handle any other error cases, not much can happen here, mostly network errors

                throw;
            }
            finally
            {
                _cartCreateLock.Release();
            }
        }

        protected override async Task HandleRequestAsync(AddProductRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                var targetProduct = await _userProductViewStateLookupService.GetStateAsync(request.ProductId, cancellationToken);

                var currentProduct = ViewState.Products.FirstOrDefault();

                // check if current line state allows or follows add as quantity logic
                if (currentProduct != null &&
                    currentProduct.ProductId == request.ProductId && (currentProduct.ProductEntityType == ProductType.Product || currentProduct.ProductEntityType == ProductType.ProductBundle))
                {
                    // we need to update quantity concurrently, this lock should not cause any performance problems, sure other approaches can be used
                    decimal newQuantity = 0;
                    lock (currentProduct)
                    {
                        newQuantity = currentProduct.Quantity + request.Quantity;
                        currentProduct.Quantity = (int)newQuantity;
                    }

                    await _cartsWebApiClient.QuantityAsync(currentCartId, currentProduct.Guid, newQuantity, cancellationToken);
                }
                else
                {
                    var entryCreateResult = await _cartsWebApiClient.AddAsync(currentCartId, new CartEntryProductAddModel()
                    {
                        PayType = OrderLinePayType.Cash,
                        Quantity = request.Quantity,
                        ProductId = request.ProductId,
                        UnitPrice = null,
                        Mark = request.Mark,
                    }, cancellationToken);

                    var productState = new UserCartProductViewState()
                    {
                        Guid = entryCreateResult.Id,
                        ProductEntityType = targetProduct.ProductType,
                        ProductId = request.ProductId,
                        PurchaseOptions = targetProduct.PurchaseOptions,
                        ProductName = targetProduct.Name,
                        Quantity = (int)request.Quantity,
                        Mark = request.Mark
                    };

                    ViewState.Add(productState);
                }
            }
            catch (WebApiClientException ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext(), cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(SetQuantityRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                await _cartsWebApiClient.QuantityAsync(currentCartId, request.EntryId, request.Quantity, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext() { EntryId = request.EntryId }, cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(SetPayTypeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                await _cartsWebApiClient.PayTypeAsync(currentCartId, request.EntryId, request.PayType, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext() { EntryId = request.EntryId }, cancellationToken);
            }
        }

        protected override Task HandleRequestAsync(AddDepositRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleRequestAsync(RemoveEntryRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                await _cartsWebApiClient.RemoveAsync(currentCartId, request.EntryId, cancellationToken);

                // entry will already be removed in preprocessor but just in case
                ViewState.TryRemoveEntry(request.EntryId);
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext() { EntryId = request.EntryId }, cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(AddPaymentRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                await _cartsWebApiClient.PaymentMethodSetAsync(currentCartId, new CartPaymentMethodSetModel()
                {
                    PaymentMethodId = request.PaymentMethodId
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext() { PaymentMethodId = request.PaymentMethodId }, cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(RemovePaymentRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                await _cartsWebApiClient.PaymentMethodSetAsync(currentCartId, new CartPaymentMethodSetModel()
                {
                    PaymentMethodId = null
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext() { PaymentMethodId = request.PaymentMethodId }, cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(AddPomoCodeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                var result = await _cartsWebApiClient.PromotionCodeAddAsync(currentCartId, new CartPromotionCodeAddModel() { Code = request.PromoCode }, cancellationToken);

                // keep the typed promo code value, all other updates to promocode value will be done based on cart state              
                ViewState.PromoCodeViewState.IsLoading = false;
                ViewState.PromoCodeViewState.PromoCode = request.PromoCode;
                ViewState.PromoCodeViewState.RaiseChanged();
            }
            catch (WebApiClientException wace)
            {
                if (wace.ErrorCode.HasValue && wace.IsExceptionCode(ExceptionCode.Promotion))
                {
                    ViewState.PromotionExceptionMessage = _assemblyResourcesLocalizationService.GetLocalizedStringValue((PromotionErrorCode)wace.ErrorCode);
                    ViewState.RaiseChanged();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext(), cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(RemovePromoCodeRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                var result = await _cartsWebApiClient.PromotionCodeRemoveAsync(currentCartId, cancellationToken);

                // reset state, this will also trigger loading=false
                ViewState.ResetPromotionState();
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext(), cancellationToken);
            }
        }

        protected override async Task HandleRequestAsync(ClearCartRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var currentCartId = await CartGetOrCreateAsync(cancellationToken);
                var result = await _cartsWebApiClient.ClearAsync(currentCartId, cancellationToken);

                ViewState.Clear();
            }
            catch (Exception ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext(), cancellationToken);
            }
        }
    }
}
