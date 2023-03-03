using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AdvertisementViewStateLookupService : ViewStateLookupServiceBase<int, AdvertisementViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AdvertisementViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AdvertisementViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.NewsGetAsync(new NewsFilter() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in clientResult.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.Title = item.Title;
                viewState.Body = item.Data;
                viewState.StartDate = item.StartDate;
                viewState.EndDate = item.EndDate;
                viewState.Url = item.Url;
                viewState.VideoUrl = item.MediaUrl;
                viewState.ThumbnailUrl = item.MediaUrl;
                viewState.CustomTemplate = false;

                AddViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<AdvertisementViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.NewsGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (clientResult is null)
                return viewState;

            viewState.Title = clientResult.Title;
            viewState.Body = clientResult.Data;
            viewState.StartDate = clientResult.StartDate;
            viewState.EndDate = clientResult.EndDate;
            viewState.Url = clientResult.Url;
            viewState.VideoUrl = clientResult.MediaUrl;
            viewState.ThumbnailUrl = clientResult.MediaUrl;
            viewState.CustomTemplate = false;

            return viewState;
        }
        protected override AdvertisementViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AdvertisementViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Body = "Default body";

            return defaultState;
        }
    }
}
