using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserProductGroupViewStateLookupService : ViewStateLookupServiceBase<int, UserProductGroupViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public UserProductGroupViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserProductGroupViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, UserProductGroupViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserProductGroupsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<UserProductGroupViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGroupGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<UserProductGroupViewState> UpdateViewStateAsync(UserProductGroupViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGroupGetAsync(viewState.ProductGroupId, cToken);
            
            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override UserProductGroupViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserProductGroupViewState>();

            defaultState.ProductGroupId = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private UserProductGroupViewState Map(UserProductGroupModel model, UserProductGroupViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.Name = model.Name;
            result.SortOption = model.SortOption;
            
            return result;
        }
        #endregion
    }
}
