using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserCartProductViewStateLookupService : ViewStateLookupServiceBase<int, UserProductViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public UserCartProductViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserCartProductViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var products = await _gizmoClient.UserProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var product in products.Data)
            {
                var viewState = CreateDefaultViewState(product.Id);

                viewState.Id = product.Id;
                viewState.UnitPrice = product.Price;
                viewState.UnitPointsPrice = product.PointsPrice;
                viewState.Name = product.Name;

                AddViewState(product.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<UserProductViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var product = await _gizmoClient.UserProductGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (product is null)
                return viewState;

            viewState.Id = lookUpkey;

            viewState.UnitPrice = product.Price;
            viewState.UnitPointsPrice = product.PointsPrice;
            viewState.Name = product.Name;

            return viewState;
        }
        protected override UserProductViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserProductViewState>();

            defaultState.Id = lookUpkey;

            return defaultState;
        }
    }
}
