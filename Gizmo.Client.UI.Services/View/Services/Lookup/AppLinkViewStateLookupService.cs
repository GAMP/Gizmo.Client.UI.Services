using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

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

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var executables = await _gizmoClient.UserApplicationLinksGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in executables.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.AppLinkId = item.Id;
                viewState.ApplicationId = item.ApplicationId;
                viewState.Url = item.Url;
                viewState.DisplayOrder = item.DisplayOrder;

                AddViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<AppLinkViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserApplicationLinkGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.AppLinkId = item.Id;
            viewState.ApplicationId = item.ApplicationId;
            viewState.Url = item.Url;
            viewState.DisplayOrder = item.DisplayOrder;

            return viewState;
        }
        protected override AppLinkViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppLinkViewState>();

            defaultState.AppLinkId = lookUpkey;

            return defaultState;
        }
    }
}
