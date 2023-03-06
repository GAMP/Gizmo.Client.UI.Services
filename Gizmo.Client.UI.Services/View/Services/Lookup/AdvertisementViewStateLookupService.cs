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

                viewState.IsCustomTemplate = item.IsCustomTemplate;
                viewState.Body = item.Data;

                if (!viewState.IsCustomTemplate)
                {
                    viewState.Title = item.Title;
                    viewState.StartDate = item.StartDate;
                    viewState.EndDate = item.EndDate;
                    viewState.Url = item.Url;
                    viewState.MediaUrl = item.MediaUrl;

                    if (!string.IsNullOrWhiteSpace(item.ThumbnailUrl))
                    {
                        viewState.ThumbnailUrl = item.ThumbnailUrl;
                        viewState.ThumbnailType = GetThumbnailType(item.ThumbnailUrl);
                    }

                    viewState.Command = GetUrlCommand(item.Url);
                }

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

            viewState.IsCustomTemplate = clientResult.IsCustomTemplate;
            viewState.Body = clientResult.Data;

            if (!viewState.IsCustomTemplate)
            {
                viewState.Title = clientResult.Title;
                viewState.StartDate = clientResult.StartDate;
                viewState.EndDate = clientResult.EndDate;
                viewState.Url = clientResult.Url;
                viewState.MediaUrl = clientResult.MediaUrl;

                if (!string.IsNullOrWhiteSpace(clientResult.ThumbnailUrl))
                {
                    viewState.ThumbnailUrl = clientResult.ThumbnailUrl;
                    viewState.ThumbnailType = GetThumbnailType(clientResult.ThumbnailUrl);
                }

                viewState.Command = GetUrlCommand(clientResult.Url);
            }

            return viewState;
        }
        protected override AdvertisementViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AdvertisementViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Body = "Default body";

            return defaultState;
        }

        private static AdvertisementCommand? GetUrlCommand(string? url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;

                if (!Uri.TryCreate(url, UriKind.Absolute, out var commandUrl))
                    throw new NotSupportedException($"Url '{url}' is not supported.");

                if (!commandUrl.Scheme.Equals("gizmo"))
                    throw new NotSupportedException($"Scheme from the '{url}' is not supported.");

                return new()
                {
                    CommandType = commandUrl.Host switch
                    {
                        "addcart" => AdvertisementCommandType.AddToCart,
                        "launch" => AdvertisementCommandType.Launch,
                        "navigate" => AdvertisementCommandType.Navigate,
                        _ => throw new NotImplementedException(commandUrl.Host),
                    },
                    Parts = commandUrl.Segments[0..^1],
                    PathId = int.Parse(commandUrl.Segments[^1])
                };
            }
            catch (Exception exeption)
            {
                throw new NotSupportedException($"Advertisement command was not recognized. {exeption.Message}");
            }
        }
        private static AdvertisementThumbnailType GetThumbnailType(string url)
        {
            try
            {
                return !Path.HasExtension(url)
                    ? throw new NotSupportedException($"Thumbnail source '{url}' had no extension.")
                    : Uri.TryCreate(url, UriKind.Absolute, out var _)
                        ? AdvertisementThumbnailType.External
                        : AdvertisementThumbnailType.Internal;
            }
            catch (Exception exeption)
            {
                throw new NotSupportedException($"Thumbnail source from '{url}' was not recognized. {exeption.Message}");
            }
        }
    }
}
