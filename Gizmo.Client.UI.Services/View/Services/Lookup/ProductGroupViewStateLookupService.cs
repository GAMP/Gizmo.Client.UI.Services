using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ProductGroupViewStateLookupService : ViewStateLookupServiceBase<int, ProductGroupViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public ProductGroupViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<ProductGroupViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var productGroups = await _gizmoClient.ProductGroupsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            if (productGroups.Data?.Any() != true)
                return false;

            var tryAddCount = 0;

            foreach (var item in productGroups.Data)
            {
                while (tryAddCount < 5)
                {
                    if (!_cache.TryAdd(item.Id, new() { Id = item.Id, Name = item.Name }))
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

        protected override async ValueTask<ProductGroupViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var productGroup = await _gizmoClient.ProductGroupGetAsync(lookUpkey, cToken);

            return productGroup is not null
                ? new() { Id = productGroup.Id, Name = productGroup.Name }
                : base.CreateDefaultViewStateAsync(lookUpkey);
        }
        protected override ProductGroupViewState CreateDefaultViewStateAsync(int lookUpkey)
        {
            return new() { Id = 0, Name = "Not found" };
        }
    }
}
