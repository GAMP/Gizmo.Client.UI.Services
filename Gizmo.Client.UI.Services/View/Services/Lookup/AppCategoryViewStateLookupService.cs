using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

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

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var categories = await _gizmoClient.UserApplicationCategoriesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in categories.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.AppCategoryId = item.Id;
                viewState.Name = item.Name;

                AddOrUpdateViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<AppCategoryViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserApplicationCategoryGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.AppCategoryId = item.Id;
            viewState.Name = item.Name;

            return viewState;
        }
        protected override AppCategoryViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppCategoryViewState>();

            defaultState.AppCategoryId = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
    }
}
