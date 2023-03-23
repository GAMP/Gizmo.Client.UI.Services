using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

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

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var groups = await _gizmoClient.UserProductGroupsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in groups.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.Name = item.Name;

                AddOrUpdateViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<UserProductGroupViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var group = await _gizmoClient.UserProductGroupGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (group is null)
                return viewState;

            viewState.Name = group.Name;

            return viewState;
        }
        protected override UserProductGroupViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserProductGroupViewState>();

            defaultState.ProductGroupId = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
    }
}
