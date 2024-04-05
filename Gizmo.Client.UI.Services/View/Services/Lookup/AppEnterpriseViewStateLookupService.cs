using Gizmo.Client.UI.Services;
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
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.AppEnterpriseChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.AppEnterpriseChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, AppEnterpriseViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserApplicationEnterprisesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppEnterpriseViewState> CreateViewStateAsync(int key, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationEnterpriseGetAsync(key, cToken);

            return clientResult is null ? CreateDefaultViewState(key) : Map(clientResult);
        }
        protected override async ValueTask<AppEnterpriseViewState> UpdateViewStateAsync(int key, AppEnterpriseViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationEnterpriseGetAsync(viewState.AppEnterpriseId, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override AppEnterpriseViewState CreateDefaultViewState(int key)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppEnterpriseViewState>();

            defaultState.AppEnterpriseId = key;

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
