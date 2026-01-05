using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Clients;
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
            UserProductViewStateLookupService userProductViewStateLookupService) : base(viewState, globalCancellationService, localizationService, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _userProductViewStateLookupService = userProductViewStateLookupService;
        }
        #endregion

        #region FIELDS

        private readonly ILocalizationService _localizationService;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;

        #endregion

        public Task<UserCartProductViewState?> GetCartProductItemViewStateAsync(int productId)
        {
            return Task.FromResult<UserCartProductViewState?>(ViewState.Products.Where(a => a.ProductId == productId).FirstOrDefault());
        }

        public override Task AcceptAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
                    // when adding product time we should block in case of being added to joined guest                  

                    //TODO: AAAAA
                }
            }
            else if (request is AddDepositRequest addDepositRequest)
            {
                // empty carts would mean adding the deposit to joined guest, we don't want that

                //TODO: AAAAA
            }
            else if (request is AddPaymentRequest addPaymentRequest)
            {
                //TODO: AAAAA

                return false;
            }

            return true;
        }

        protected override async Task<ICartRequest> PreProcessRequestAsync(ICartRequest request, CancellationToken cancellationToken)
        {
            if (request is SetQuantityRequest setQuantityRequest)
            {
                if (ViewState.TryGetProductEntryViewState(setQuantityRequest.EntryId, out var productEntryViewState))
                {
                    //TODO: AAAAA
                    //if (productEntryViewState.ProductType == ProductTypes.Product)
                    //{
                    //    productEntryViewState.Quantity = (int)setQuantityRequest.Quantity;
                    //    productEntryViewState.RaiseChanged();
                    //}
                }
            }
            else if (request is SetCustomPriceRequest setCustomPriceRequest)
            {
                if (ViewState.TryGetProductEntryViewState(setCustomPriceRequest.EntryId, out var productEntryViewState))
                {
                    productEntryViewState.IsCustomPrice = true;
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
            else if (request is AddProductRequest || request is AddDepositRequest)
            {
                //TODO: AAAAA
            }
            else if (request is ClearCartRequest)
            {
                //TODO: AAAAA
                //ViewState.Clear();
                //ViewState.RaiseChanged();
            }
            else if (request is AddPomoCodeRequest || request is RemovePromoCodeRequest)
            {
                ViewState.PromoCodeViewState.IsLoading = true;
                ViewState.PromoCodeViewState.RaiseChanged();
            }

            return await ValueTask.FromResult(request);
        }

        protected override ValueTask HandleRequestErrorAsync(Exception exception, ICartRequest request, CartRequestContext cartRequestContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override async ValueTask HandleProcessingErrorAsync(Exception exception, CancellationToken cancellationToken = default)
        {
            if (exception is WebApiClientException webApiClientException)
            {
                //TODO: AAAAA
            }
        }

        protected override Task RefreshStateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task<Guid> CartGetOrCreateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleRequestAsync(AddProductRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var targetProduct = await _userProductViewStateLookupService.GetStateAsync(request.ProductId, cancellationToken);

                var productState = new UserCartProductViewState()
                {
                    Guid = Guid.NewGuid(),
                    //TODO: AAAAA MOVE ProductTypes ENUM IN GIZMO.SHARED? ProductType = ProductTypes.Product,
                    ProductEntityType = targetProduct.ProductType,
                    ProductId = request.ProductId,
                    PurchaseOptions = targetProduct.PurchaseOptions,
                    ProductName = targetProduct.Name,
                    Quantity = (int)request.Quantity,
                    Mark = request.Mark
                };
            }
            catch (WebApiClientException ex)
            {
                await HandleRequestErrorAsync(ex, request, new CartRequestContext(), cancellationToken);
            }
        }

        protected override Task HandleRequestAsync(SetQuantityRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(SetPayTypeRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(AddDepositRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(RemoveEntryRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(AddPaymentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(RemovePaymentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(AddPomoCodeRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(RemovePromoCodeRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        protected override Task HandleRequestAsync(ClearCartRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}
