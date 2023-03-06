using System;
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
                    viewState.Title = item.Title;
                    viewState.StartDate = item.StartDate;
                    viewState.EndDate = item.EndDate;

                    (viewState.ThumbnailType, viewState.ThumbnailUrl) = GetThumbnailInfo(item.ThumbnailUrl, item.MediaUrl);
                    (viewState.Url, viewState.Command) = GetUrlInfo(item.Url);
                    viewState.MediaUrl = viewState.ThumbnailType == AdvertisementThumbnailType.None ? null : item.MediaUrl;
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

                (viewState.ThumbnailType, viewState.ThumbnailUrl) = GetThumbnailInfo(clientResult.ThumbnailUrl, clientResult.MediaUrl);

                (viewState.Url, viewState.Command) = GetUrlInfo(clientResult.Url);
                
                viewState.MediaUrl = viewState.ThumbnailType == AdvertisementThumbnailType.None ? null : clientResult.MediaUrl;
            }

            return viewState;
        }
        protected override AdvertisementViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AdvertisementViewState>();

            defaultState.Id = lookUpkey;
            defaultState.Body = "DEFAULT";

            return defaultState;
        }

        private static (string? Url, AdvertisementCommand? Command) GetUrlInfo(string? url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return (null, null);

            if (!uri.Scheme.Equals("gizmo"))
                return (uri.AbsoluteUri, null);

            try
            {
                var command = new AdvertisementCommand()
                {
                    CommandType = uri.Host switch
                    {
                        "addcart" => AdvertisementCommandType.AddToCart,
                        "launch" => AdvertisementCommandType.Launch,
                        "navigate" => AdvertisementCommandType.Navigate,
                        _ => throw new NotSupportedException(uri.Host)
                    },
                    Parts = uri.Segments[0..^1],
                    PathId = int.Parse(uri.Segments[^1])
                };

                return (uri.AbsoluteUri, command);
            }
            catch (Exception exeption)
            {
                throw new NotSupportedException($"Advertisement command was not recognized. {exeption.Message}");
            }
        }
        private static (AdvertisementThumbnailType ThumbnailType, string? ThumbnailUrl) GetThumbnailInfo(string? thumbnailUrl, string? mediaUrl)
        {
            if (Uri.TryCreate(thumbnailUrl, UriKind.Absolute, out _))
                return (AdvertisementThumbnailType.Manually, thumbnailUrl);

            if (string.IsNullOrWhiteSpace(mediaUrl))
                return (AdvertisementThumbnailType.None, null);

            if (!Uri.TryCreate(mediaUrl, UriKind.Absolute, out var mediaUri))
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
