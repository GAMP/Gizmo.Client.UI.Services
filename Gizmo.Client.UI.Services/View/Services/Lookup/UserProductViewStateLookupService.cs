using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserProductViewStateLookupService : ViewStateLookupServiceBase<int, UserProductViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public UserProductViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserProductViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, UserProductViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<UserProductViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<UserProductViewState> UpdateViewStateAsync(UserProductViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGetAsync(viewState.Id, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override UserProductViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserProductViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default namer";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private UserProductViewState Map(UserProductModel model, UserProductViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            result.Name = model.Name;
            result.ProductGroupId = model.ProductGroupId;
            result.Description = model.Description;
            result.ProductType = model.ProductType;
            result.UnitPrice = model.Price;
            result.UnitPointsPrice = model.PointsPrice;
            result.UnitPointsAward = model.PointsAward;
            result.DefaultImageId = model.DefaultImageId;
            result.PurchaseOptions = model.PurchaseOptions;

            if (model.ProductType == ProductType.ProductBundle)
            {
                result.BundledProducts = model.Bundle?.BundledProducts.Select(bundle =>
                {
                    var bundledProductResult = ServiceProvider.GetRequiredService<UserProductBundledViewState>();
                    bundledProductResult.Id = bundle.ProductId;
                    bundledProductResult.Quantity = bundle.Quantity;
                    return bundledProductResult;
                }).ToList() ?? Enumerable.Empty<UserProductBundledViewState>();
            }
            else if (model.ProductType == ProductType.ProductTime)
            {
                if (model.TimeProduct != null)
                {
                    var timeProductResult = ServiceProvider.GetRequiredService<UserProductTimeViewState>();
                    timeProductResult.Minutes = model.TimeProduct.Minutes;
                    result.TimeProduct = timeProductResult;
                }
            }

            return result;
        }
        #endregion
    }
}
