using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppCategoryViewStateLookupService : ViewStateLookupServiceBase<int, AppCategoryViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AppCategoryViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppCategoryViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.AppCategoryChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.AppCategoryChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, AppCategoryViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserApplicationCategoriesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppCategoryViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationCategoryGetAsync(lookUpkey, cToken);

           return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppCategoryViewState> UpdateViewStateAsync(AppCategoryViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationCategoryGetAsync(viewState.AppCategoryId, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override AppCategoryViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppCategoryViewState>();

            defaultState.AppCategoryId = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AppCategoryViewState Map (UserApplicationCategoryModel model, AppCategoryViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.Name = model.Name;
            
            return result;
        }

        #endregion
    }
}
