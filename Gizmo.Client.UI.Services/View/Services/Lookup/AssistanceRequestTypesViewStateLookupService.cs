using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AssistanceRequestTypesViewStateLookupService : ViewStateLookupServiceBase<int, AssistanceRequestTypeViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AssistanceRequestTypesViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AssistanceRequestTypesViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, AssistanceRequestTypeViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.AssistanceRequestTypesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AssistanceRequestTypeViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.AssistanceRequestTypeGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AssistanceRequestTypeViewState> UpdateViewStateAsync(AssistanceRequestTypeViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.AssistanceRequestTypeGetAsync(viewState.Id, cToken);

            return clientResult is null ? CreateDefaultViewState(viewState.Id) : Map(clientResult, viewState);
        }
        protected override AssistanceRequestTypeViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AssistanceRequestTypeViewState>();

            defaultState.Id = lookUpkey;

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AssistanceRequestTypeViewState Map(AssistanceRequestTypeModel model, AssistanceRequestTypeViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.Title = model.Title;
            result.DisplayOrder = model.DisplayOrder;
            result.IsDeleted = model.IsDeleted;

            return result;
        }
        #endregion
    }
}
