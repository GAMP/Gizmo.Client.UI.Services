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
                    var (midiaUrlType, mediaUri) = ParseMediaUrl(item.MediaUrl);
                    viewState.MediaUrlType = midiaUrlType;
                    viewState.MediaUrl = mediaUri?.AbsoluteUri;

                    viewState.ThumbnailUrl = ParseThumbnailUrl(item.ThumbnailUrl, midiaUrlType, mediaUri);

                    (viewState.Url, viewState.Command) = ParseUrl(item.Url);

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
                var (midiaUrlType, mediaUri) = ParseMediaUrl(clientResult.MediaUrl);
                viewState.MediaUrlType = midiaUrlType;
                viewState.MediaUrl = mediaUri?.AbsoluteUri;

                viewState.ThumbnailUrl = ParseThumbnailUrl(clientResult.ThumbnailUrl, midiaUrlType, mediaUri);

                (viewState.Url, viewState.Command) = ParseUrl(clientResult.Url);

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
            defaultState.MediaUrlType = AdvertisementMediaUrlType.None;

            return defaultState;
        }

        private static (AdvertisementMediaUrlType MediaUrlType, Uri? MediaUri) ParseMediaUrl(string? mediaUrl)
        {
            if (!Uri.TryCreate(mediaUrl, UriKind.Absolute, out var mediaUri))
                return (AdvertisementMediaUrlType.None, null);

            var mediaUrlType = mediaUri.Host switch
            {
                "www.youtube.com" => AdvertisementMediaUrlType.YouTube,
                "www.vk.ru" => AdvertisementMediaUrlType.Vk,
                _ => AdvertisementMediaUrlType.Custom
            };

            switch (mediaUrlType)
            {
                case AdvertisementMediaUrlType.YouTube:
                    var query = HttpUtility.ParseQueryString(mediaUri.Query);
                    var videoId = query.AllKeys.Contains("v") ? query["v"] : mediaUri.Segments[^1];
                    var url = $"https://www.youtube.com/embed/{videoId}?autoplay=1";

                    return (mediaUrlType, new Uri(url));

                default:
                    return (mediaUrlType, mediaUri);
            }
        }
        private static string? ParseThumbnailUrl(string? thumbnailUrl, AdvertisementMediaUrlType mediaUrlType, Uri? mediaUri)
        {
            if (Uri.TryCreate(thumbnailUrl, UriKind.Absolute, out _))
                return thumbnailUrl;

            if (mediaUri is null)
                return null;

            switch (mediaUrlType)
            {
                case AdvertisementMediaUrlType.YouTube:
                    var videoId = mediaUri.Segments[^1];
                    return $"https://i3.ytimg.com/vi/{videoId}/maxresdefault.jpg";

                default:
                    return null;
            }
        }
        private static (string? Url, AdvertisementCommand? Command) ParseUrl(string? url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return (null, null);

            if (!uri.Scheme.Equals("gizmo"))
                return (uri.AbsoluteUri, null);

            AdvertisementCommandType? commandType = uri.Host switch
            {
                "addcart" => AdvertisementCommandType.AddToCart,
                "launch" => AdvertisementCommandType.Launch,
                "navigate" => AdvertisementCommandType.Navigate,
                _ => null
            };

            if (!commandType.HasValue)
                return (null, null);

            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 1)
                return (null, null);

            var command = new AdvertisementCommand()
            {
                CommandType = commandType.Value,
                Parts = segments[0..^1],
                PathId = int.Parse(segments[^1])
            };

            return (null, command);
        }
    }
}
