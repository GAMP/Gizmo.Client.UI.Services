using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserCartProductItemViewStateLookupService : ViewStateLookupServiceBase<int, UserCartProductItemViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public UserCartProductItemViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserCartProductItemViewStateLookupService> logger,
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

                viewState.ProductId = product.Id;
                
                AddViewState(product.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<UserCartProductItemViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var product = await _gizmoClient.UserProductGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (product is null)
                return viewState;

            viewState.ProductId = lookUpkey;

            return viewState;
        }
        protected override UserCartProductItemViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserCartProductItemViewState>();

            defaultState.ProductId = lookUpkey;

            return defaultState;
        }
    }
}
