using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppEnterpriseViewStateLookupService : ViewStateLookupServiceBase<int, AppEnterpriseViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AppEnterpriseViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppEnterpriseViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDE FUNCTIONS
        protected override async Task<IDictionary<int, AppEnterpriseViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserApplicationEnterprisesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppEnterpriseViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationEnterpriseGetAsync(lookUpkey, cToken);
             
            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppEnterpriseViewState> UpdateViewStateAsync(AppEnterpriseViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationEnterpriseGetAsync(viewState.AppEnterpriseId, cToken);
            
            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override AppEnterpriseViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppEnterpriseViewState>();

            defaultState.AppEnterpriseId = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AppEnterpriseViewState Map(UserApplicationEnterpriseModel model, AppEnterpriseViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            result.Name = model.Name;
            
            return result;
        }
        #endregion
    }
}
