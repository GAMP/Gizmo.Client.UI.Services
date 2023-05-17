using System.Web;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

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

        #region OVERRIDED FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.AppLinkChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.AppLinkChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, AppLinkViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserApplicationLinksGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppLinkViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationLinkGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppLinkViewState> UpdateViewStateAsync(AppLinkViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationLinkGetAsync(viewState.AppLinkId, cToken);
          
            return clientResult is null ? CreateDefaultViewState(viewState.AppLinkId) : Map(clientResult, viewState);
        }
        protected override AppLinkViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppLinkViewState>();

            defaultState.AppLinkId = lookUpkey;

            return defaultState;
        }
        #endregion

        private static (AdvertisementMediaUrlType MediaUrlType, Uri? MediaUri) ParseMediaUrl(string? mediaUrl)
        {
            if (!Uri.TryCreate(mediaUrl, UriKind.Absolute, out var mediaUri))
                return (AdvertisementMediaUrlType.None, null);

            var mediaUrlType = mediaUri.Host switch
            {
                "www.youtube.com" => AdvertisementMediaUrlType.YouTube,
                "vk.com" => AdvertisementMediaUrlType.Vk,
                _ => AdvertisementMediaUrlType.None
            };

            switch (mediaUrlType)
            {
                case AdvertisementMediaUrlType.YouTube:
                    {
                        var query = HttpUtility.ParseQueryString(mediaUri.Query);
                        var videoId = query.AllKeys.Contains("v") ? query["v"] : mediaUri.Segments[^1];
                        var url = $"https://www.youtube.com/embed/{videoId}?autoplay=1";

                        return (mediaUrlType, new Uri(url));
                    }
                default:
                    return (mediaUrlType, mediaUri);
            }
        }

        private static string? ParseThumbnailUrl(AdvertisementMediaUrlType mediaUrlType, Uri? mediaUri)
        {
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

        #region PRIVATE FUNCTIONS
        private AppLinkViewState Map(UserApplicationLinkModel model, AppLinkViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            result.ApplicationId = model.ApplicationId;
            result.Caption = model.Caption;
            result.Description = model.Description;
            result.Url = model.Url;
            result.DisplayOrder = model.DisplayOrder;

            var (midiaUrlType, mediaUri) = ParseMediaUrl(model.Url);
            result.MediaUrlType = midiaUrlType;
            result.MediaUrl = mediaUri?.AbsoluteUri;

            result.ThumbnailUrl = ParseThumbnailUrl(midiaUrlType, mediaUri);

            return result;
        }
        #endregion
    }
}
