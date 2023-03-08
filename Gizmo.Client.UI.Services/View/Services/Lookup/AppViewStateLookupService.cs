using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppViewStateLookupService : ViewStateLookupServiceBase<int, AppViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public AppViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var applications = await _gizmoClient.UserApplicationsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in applications.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.ApplicationId = item.Id;
                viewState.Title = item.Title;
                viewState.Description = item.Description;
                viewState.ApplicationCategoryId = item.ApplicationCategoryId;
                viewState.ReleaseDate = item.ReleaseDate;
                viewState.AddDate = item.AddDate;
                viewState.DeveloperId = item.DeveloperId;
                viewState.PublisherId = item.PublisherId;

                AddViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<AppViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserApplicationGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.ApplicationId = item.Id;
            viewState.Title = item.Title;
            viewState.Description = item.Description;
            viewState.ApplicationCategoryId = item.ApplicationCategoryId;
            viewState.ReleaseDate = item.ReleaseDate;
            viewState.AddDate = item.AddDate;
            viewState.DeveloperId = item.DeveloperId;
            viewState.PublisherId = item.PublisherId;

            return viewState;
        }
        protected override AppViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppViewState>();

            defaultState.ApplicationId = lookUpkey;

            defaultState.Title = "Default title";

            return defaultState;
        }
    }
}
