using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppEnterprisesViewStateLookupService : ViewStateLookupServiceBase<int, AppEnterpriseViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AppEnterprisesViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppEnterprisesViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var enterprises = await _gizmoClient.UserApplicationEnterprisesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in enterprises.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.AppEnterpriseId = item.Id;
                viewState.Name = item.Name;

                AddViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<AppEnterpriseViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserApplicationEnterpriseGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.AppEnterpriseId = item.Id;
            viewState.Name = item.Name;

            return viewState;
        }
        protected override AppEnterpriseViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppEnterpriseViewState>();

            defaultState.AppEnterpriseId = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
    }
}
