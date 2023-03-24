using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

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

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, UserCartProductItemViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<UserCartProductItemViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map( clientResult);
        }
        protected override async ValueTask<UserCartProductItemViewState> UpdateViewStateAsync(UserCartProductItemViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGetAsync(viewState.ProductId, cToken);
            
            return clientResult is null ? viewState : Map( clientResult, viewState);
        }
        protected override UserCartProductItemViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserCartProductItemViewState>();

            defaultState.ProductId = lookUpkey;

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private UserCartProductItemViewState Map(UserProductModel model, UserCartProductItemViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            return result;
        }
        #endregion
    }
}
