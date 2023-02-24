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
            var productGroups = await _gizmoClient.UserProductGroupsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            var tryAddCount = 0;

            foreach (var item in productGroups.Data)
            {
                while (tryAddCount < 5)
                {
                    if (!_cache.TryAdd(item.Id, new() { ProductGroupId = item.Id, Name = item.Name }))
                    {
                        tryAddCount++;
                    }
                    else
                    {
                        tryAddCount = 0;
                        break;
                    }
                }

                if (tryAddCount != 0)
                    return false;
            }

            return true;
        }
        protected override async ValueTask<UserProductGroupViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var productGroup = await _gizmoClient.UserProductGroupGetAsync(lookUpkey, cToken);

            return productGroup is not null
                ? new() { ProductGroupId = productGroup.Id, Name = productGroup.Name }
                : base.CreateDefaultViewStateAsync(lookUpkey);
        }
        protected override UserProductGroupViewState CreateDefaultViewStateAsync(int lookUpkey) =>
             base.CreateDefaultViewStateAsync(lookUpkey);
    }
}
