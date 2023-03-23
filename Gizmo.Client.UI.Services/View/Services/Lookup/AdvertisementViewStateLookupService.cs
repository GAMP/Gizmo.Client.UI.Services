using System.Reactive.Linq;
using System.Web;

using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services;

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

    #region EVENT HANDLER
    private async void OnNewsChangeAsync(object? sender, NewsEventArgs args)
    {
        await HandleChangesAsync(args.EntityId, args.ModificationType);
        RaiseChanged(args.ModificationType);
    }
    #endregion

    #region OVERRIDED FUNCTIONS
    protected override Task OnInitializing(CancellationToken ct)
    {
        _gizmoClient.NewsChange += OnNewsChangeAsync;
        return base.OnInitializing(ct);
    }
    protected override void OnDisposing(bool isDisposing)
    {
        _gizmoClient.NewsChange -= OnNewsChangeAsync;
        base.OnDisposing(isDisposing);
    }

    protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
    {
        var clientResult = await _gizmoClient.NewsGetAsync(new NewsFilter() { Pagination = new() { Limit = -1 } }, cToken);

        foreach (var item in clientResult.Data)
            AddOrUpdateViewState(item.Id, Map(item.Id, item));

        return true;
    }
    protected override async ValueTask<AdvertisementViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
    {
        var clientResult = await _gizmoClient.NewsGetAsync(lookUpkey, cToken);

        return clientResult is null
            ? CreateDefaultViewState(lookUpkey)
            : Map(lookUpkey, clientResult);
    }
    protected override AdvertisementViewState CreateDefaultViewState(int lookUpkey)
    {
        var defaultState = ServiceProvider.GetRequiredService<AdvertisementViewState>();

        defaultState.Id = lookUpkey;
        defaultState.Body = "<div style=\"max-width: 40.0rem; margin: 8.6rem 3.2rem 6.5rem 3.2rem\">DEFAULT BODY</div>";
        defaultState.MediaUrlType = AdvertisementMediaUrlType.None;

        return defaultState;
    }
    #endregion

    #region PRIVATE FUNCTIONS
    private AdvertisementViewState Map(int lookUpkey, NewsModel model)
    {
        var viewState = CreateDefaultViewState(lookUpkey);

        viewState.IsCustomTemplate = model.IsCustomTemplate;
        viewState.Body = model.Data;

        if (!viewState.IsCustomTemplate)
        {
            var (midiaUrlType, mediaUri) = ParseMediaUrl(model.MediaUrl);
            viewState.MediaUrlType = midiaUrlType;
            viewState.MediaUrl = mediaUri?.AbsoluteUri;

            viewState.ThumbnailUrl = ParseThumbnailUrl(model.ThumbnailUrl, midiaUrlType, mediaUri);

            (viewState.Url, viewState.Command) = ParseUrl(model.Url);

            viewState.Title = model.Title;
            viewState.StartDate = model.StartDate;
            viewState.EndDate = model.EndDate;
        }

        return viewState;
    }
    private static (AdvertisementMediaUrlType MediaUrlType, Uri? MediaUri) ParseMediaUrl(string? mediaUrl)
    {
        if (!Uri.TryCreate(mediaUrl, UriKind.Absolute, out var mediaUri))
            return (AdvertisementMediaUrlType.None, null);

        var mediaUrlType = mediaUri.Host switch
        {
            "www.youtube.com" => AdvertisementMediaUrlType.YouTube,
            "vk.com" => AdvertisementMediaUrlType.Vk,
            _ => AdvertisementMediaUrlType.Custom
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
    private static (string? Url, ViewServiceCommand? Command) ParseUrl(string? url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return (null, null);

        if (!uri.Scheme.Equals("gizmo", StringComparison.OrdinalIgnoreCase))
            return (uri.AbsoluteUri, null);

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 1)
            return (null, null);

        var commandName = segments[^1].ToLower();

        ViewServiceCommandType? commandType = commandName switch
        {
            "add" => ViewServiceCommandType.Add,
            "delete" => ViewServiceCommandType.Delete,
            "launch" => ViewServiceCommandType.Launch,
            "navigate" => ViewServiceCommandType.Navigate,
            _ => null
        };

        if (!commandType.HasValue)
            return (null, null);

        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        var commandParams = new Dictionary<string, object>(queryParams.Count, StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < queryParams.Count; i++)
        {
            var paramKey = queryParams.GetKey(i);

            if (paramKey is null)
                continue;

            var paramValues = queryParams.GetValues(paramKey);

            if (paramValues?.Any() == true)
            {
                if (paramValues.Length == 1)
                    commandParams.Add(paramKey, paramValues[0]);
                else
                    continue;
            }
        }

        var routeKey = string.Join("/", segments[0..^1].Prepend(uri.Host)).ToLower();

        var command = new ViewServiceCommand()
        {
            Key = routeKey,
            Name = routeKey + '/' + commandName,
            Type = commandType.Value,
            Params = commandParams
        };

        return (null, command);
    }
    #endregion
}
