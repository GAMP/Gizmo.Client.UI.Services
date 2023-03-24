using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppLinkViewStateLookupService : ViewStateLookupServiceBase<int, AppLinkViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AppLinkViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppLinkViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, AppLinkViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserApplicationLinksGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppLinkViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationLinkGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppLinkViewState> UpdateViewStateAsync(AppLinkViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationLinkGetAsync(viewState.AppLinkId, cToken);
          
            return clientResult is null ? CreateDefaultViewState(viewState.AppLinkId) : Map(clientResult, viewState);
        }
        protected override AppLinkViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppLinkViewState>();

            defaultState.AppLinkId = lookUpkey;

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AppLinkViewState Map(UserApplicationLinkModel model, AppLinkViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            result.ApplicationId = model.ApplicationId;
            result.Url = model.Url;
            result.DisplayOrder = model.DisplayOrder;
            
            return result;
        }
        #endregion
    }
}
