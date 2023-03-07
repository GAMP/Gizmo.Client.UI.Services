using System.Web;

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
                    var mediaUrl = ParseMediaUrl(item.MediaUrl);
                    viewState.MediaUrl = mediaUrl?.AbsoluteUri;

                    (viewState.Url, viewState.Command) = ParseUrl(item.Url);
                    (viewState.ThumbnailType, viewState.ThumbnailUrl) = ParseThumbnail(item.ThumbnailUrl, mediaUrl);

                    viewState.Title = item.Title;
                    viewState.StartDate = item.StartDate;
                    viewState.EndDate = item.EndDate;
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
                var mediaUrl = ParseMediaUrl(clientResult.MediaUrl);
                viewState.MediaUrl = mediaUrl?.AbsoluteUri;

                (viewState.Url, viewState.Command) = ParseUrl(clientResult.Url);
                (viewState.ThumbnailType, viewState.ThumbnailUrl) = ParseThumbnail(clientResult.ThumbnailUrl, mediaUrl);

                viewState.Title = clientResult.Title;
                viewState.StartDate = clientResult.StartDate;
                viewState.EndDate = clientResult.EndDate;
            }

            return viewState;
        }
        protected override AdvertisementViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AdvertisementViewState>();

            defaultState.Id = lookUpkey;
            defaultState.Body = "<div style=\"max-width: 40.0rem; margin: 8.6rem 3.2rem 6.5rem 3.2rem\">DEFAULT BODY</div>";
            defaultState.ThumbnailType = AdvertisementThumbnailType.None;

            return defaultState;
        }

        private static Uri? ParseMediaUrl(string? mediaUrl) =>
            !Uri.TryCreate(mediaUrl, UriKind.Absolute, out var result) ? null : result;
        private static (string? Url, AdvertisementCommand? Command) ParseUrl(string? url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return (null, null);

            if (!uri.Scheme.Equals("gizmo"))
                return (uri.AbsoluteUri, null);

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 1)
                return (null, null);

            var command = new AdvertisementCommand()
            {
                CommandType = uri.Host switch
                {
                    "addcart" => AdvertisementCommandType.AddToCart,
                    "launch" => AdvertisementCommandType.Launch,
                    "navigate" => AdvertisementCommandType.Navigate,
                    _ => AdvertisementCommandType.NotSupported
                },
                Parts = segments[0..^1],
                PathId = int.Parse(segments[^1])
            };

            return (null, command);
        }
        private static (AdvertisementThumbnailType ThumbnailType, string? ThumbnailUrl) ParseThumbnail(string? thumbnailUrl, Uri? mediaUri)
        {
            if (Uri.TryCreate(thumbnailUrl, UriKind.Absolute, out _))
                return (AdvertisementThumbnailType.Manually, thumbnailUrl);

            if (mediaUri is null)
                return (AdvertisementThumbnailType.None, null);

            var thumbnailUrlType = mediaUri.Host switch
            {
                "www.youtube.com" => AdvertisementThumbnailType.YouTube,
                "www.vk.ru" => AdvertisementThumbnailType.Vk,
                _ => AdvertisementThumbnailType.None
            };

            if (thumbnailUrlType == AdvertisementThumbnailType.None)
                return (AdvertisementThumbnailType.None, null);

            var query = HttpUtility.ParseQueryString(mediaUri.Query);

            switch (thumbnailUrlType)
            {
                case AdvertisementThumbnailType.YouTube:
                    {
                        var videoId = query.AllKeys.Contains("v") ? query["v"] : mediaUri.Segments[^1];
                        return (AdvertisementThumbnailType.YouTube, $"https://i3.ytimg.com/vi/{videoId}/maxresdefault.jpg");
                    }
                default:
                    {
                        return (AdvertisementThumbnailType.None, mediaUri.AbsoluteUri);
                    }
            }

        }
    }
}
