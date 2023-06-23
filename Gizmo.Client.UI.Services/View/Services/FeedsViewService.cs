using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using Gizmo.Client.UI;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.View.Services
{
    [Route(ClientRoutes.HomeRoute)]
    [Route(ClientRoutes.ApplicationsRoute)]
    [Route(ClientRoutes.ShopRoute)]
    [Register()]
    public sealed class FeedsViewService : ViewStateServiceBase<FeedsViewState>
    {
        public FeedsViewService(FeedsViewState viewState,
            IGizmoClient gizmoClient,
            IOptionsMonitor<FeedsOptions> feedOptions,
            IHttpClientFactory httpClientFactory,
            ILogger<FeedsViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _httpClientFactory = httpClientFactory;
            _feedOptions = feedOptions;
        }

        private const string HTTP_CLIENT_NAME = "FeedHttpClient";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGizmoClient _gizmoClient;
        private readonly SemaphoreSlim _initLock = new(1);
        private Timer? _rotatateTimer;
        private readonly Dictionary<FeedViewState, FeedChannelViewState> _feedLookup = new();
        private readonly object _rotateLock = new();
        private int _currentFeedIndex = 0;
        private bool _isPaused = false;
        private readonly IOptionsMonitor<FeedsOptions> _feedOptions;

        /// <summary>
        /// Gets if rotation is currently paused.
        /// </summary>
        public bool IsRotationPaused
        {
            get { return _isPaused; }
        }

        /// <summary>
        /// Pauses rotation.
        /// </summary>
        /// <param name="pause">Pause value.</param>
        public Task PauseAsync(bool pause)
        {
            _isPaused = pause;
            if (!pause)
            {
                _rotatateTimer?.Change(GetRotateMills(), GetRotateMills()); //need to add global options 
            }
            return Task.CompletedTask;
        }

        private int GetRotateMills()
        {
            if (_feedOptions.CurrentValue.RotateEvery <= 0)
                return 6000; //just in case check that correct value is set and provide default value if not

            return (int)TimeSpan.FromSeconds(_feedOptions.CurrentValue.RotateEvery).TotalMilliseconds;
        }

        private async Task InitializeIfRequired(CancellationToken cancellationToken)
        {
            if (ViewState.IsInitialized == true)
                return;

            try
            {
                if (await _initLock.WaitAsync(TimeSpan.Zero, cancellationToken))
                {
                    try
                    {
                        if (ViewState.IsInitialized == true)
                            return;

                        ViewState.IsInitializing = true;
                        DebounceViewStateChanged();

                        var feeds = await _gizmoClient.FeedsGetAsync(new Web.Api.Models.FeedsFilter()
                        {
                            Pagination = new Web.Api.Models.ModelFilterPagination()
                            {
                                Limit = -1
                            }
                        }, cancellationToken);

                        foreach (var feed in feeds.Data)
                        {
                            //null check
                            if (string.IsNullOrEmpty(feed.Url))
                                continue;

                            //try parse
                            if (!Uri.TryCreate(feed.Url, UriKind.Absolute, out Uri? uri))
                                continue;

                            try
                            {
                                var (Channel, Items) = await CreateAsync(uri, cancellationToken);

                                //create maximum of feeds
                                var maxFeeds = feed.Maximum <= 0 ? int.MaxValue : feed.Maximum;

                                //create items query
                                var itemsQuery = Items.Take(maxFeeds).OrderBy(item => item.PublishDate);

                                foreach (var item in itemsQuery)
                                {
                                    _feedLookup.Add(item, Channel);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(ex, "Feed creation failed for {feedUrl}", uri);
                            }
                        }

                        ViewState.IsInitialized = true;

                        DebounceViewStateChanged();

                        _rotatateTimer?.Change(0, GetRotateMills());
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        _initLock.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Feeds initialization failed.");
            }
            finally
            {
                ViewState.IsInitializing = false;
            }
        }

        private async Task<(FeedChannelViewState Channel, IEnumerable<FeedViewState> Items)> CreateAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            //create http client
            var httpClient = _httpClientFactory.CreateClient(HTTP_CLIENT_NAME);

            //get response
            var response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            //ensure success response
            response.EnsureSuccessStatusCode();

            //process feed stream
            using (var feedStream = response.Content.ReadAsStream(cancellationToken))
            {
                using (var xmlReader = XmlReader.Create(feedStream))
                {
                    var feedSource = SyndicationFeed.Load(xmlReader);

                    var channel = ServiceProvider.GetRequiredService<FeedChannelViewState>();
                    channel.Url = feedSource.BaseUri?.ToString();
                    channel.Description = feedSource.Description?.Text;
                    channel.Image.Url = feedSource.ImageUrl?.ToString();

                    var channelItems = feedSource.Items.Select((x, y) =>
                    {
                        var viewState = ServiceProvider.GetRequiredService<FeedViewState>();

                        //title is provided in most cases
                        viewState.Title = x.Title?.Text ?? string.Empty;

                        //summary, might not always be presents
                        viewState.Summary = x.Summary?.Text ?? string.Empty; //ue default values

                        //published data, might not always be presents
                        viewState.PublishDate = x.PublishDate.LocalDateTime;

                        //content, might not always be presents
                        viewState.Content = x.Content?.ToString(); //use default values         

                        //element extensions might contain multiple content values such as media,content etc.
                        var elementExtensions = x.ElementExtensions
                            .Select(element => element.GetObject<XElement>())
                            .ToList();

                        //check if content is provided or try to obtain from extensions
                        viewState.Content ??= elementExtensions
                        .Where(x => x.Name.NamespaceName == "http://purl.org/rss/1.0/modules/content/")
                        .FirstOrDefault()?.Value;

                        //produce default optional link
                        viewState.Link.Url = x.Links.FirstOrDefault()?.Uri?.ToString();

                        //produce images, wthis will either parse media or image sections to produce
                        //optional single default image url
                        viewState.Image.Url = elementExtensions
                        .Select(element => ParseNodeForImage(element))
                        .Where(result => result.Success)
                        .Select(image => new FeedImageViewState()
                        {
                            Url = image.ImageUrl!
                        }).FirstOrDefault()?.Url;

                        //use title in case summary does not have any value
                        if (string.IsNullOrWhiteSpace(viewState.Summary))
                        {
                            viewState.Summary = viewState.Title;
                        }

                        return viewState;
                    }).ToList();

                    return (channel, channelItems);
                }
            }
        }

        private static (bool Success, string? ImageUrl) ParseNodeForImage(XElement xElement)
        {
            //try to obtain image with url attribute
            var urlAttribute = xElement.Attribute("url");
            var imageTypeAttribute = xElement.Attribute("type");

            if (urlAttribute != null && !string.IsNullOrWhiteSpace(urlAttribute.Value))
            {
                if (imageTypeAttribute != null && imageTypeAttribute.Value == "image/jpeg")
                {
                    return (true, urlAttribute.Value);
                }
            }

            //try to obtain image directly from the xml element
            if (xElement.Name == "image")
            {
                return (true, xElement.Value);
            }

            return (false, null);
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            _rotatateTimer?.Dispose();
            base.OnDisposing(isDisposing);
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.IsEnabled = !_feedOptions.CurrentValue.Disabled;

            if (ViewState.IsEnabled)
            {
                await InitializeIfRequired(cancellationToken);
            }

            await base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            try
            {
                switch (e.State)
                {
                    case LoginState.LoggedIn:
                        _rotatateTimer?.Dispose();
                        _rotatateTimer = new Timer(OnTimerCallback, null, 0, GetRotateMills());
                        break;
                    case LoginState.LoggingOut:
                        _rotatateTimer?.Dispose();
                        _rotatateTimer = null;
                        _currentFeedIndex = 0; //reset feed index
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to handle user login state change event.");
            }
        }

        private void OnTimerCallback(object? state)
        {
            if (IsRotationPaused)
                return;

            if (Monitor.TryEnter(_rotateLock))
            {
                try
                {
                    //check if view state is initialized
                    if (ViewState.IsInitialized != true)
                        return;

                    //check if there are any feed channels present
                    if (!_feedLookup.Any())
                        return;

                    if (_currentFeedIndex >= _feedLookup.Count)
                    {
                        _currentFeedIndex = 0;
                    }

                    _currentFeedIndex = new Random().Next(0, _feedLookup.Count - 1);

                    Logger.LogTrace("Current feed index {feedIndex}", _currentFeedIndex);

                    var pair = _feedLookup.ElementAt(_currentFeedIndex);

                    Logger.LogTrace("Current feed [{title}] Image [{image}] Channel [{channel}]", pair.Key.Title, pair.Key.Image.Url, pair.Value.Description);

                    ViewState.CurrentFeedChannel = pair.Value;
                    ViewState.CurrentFeed = pair.Key;

                    _currentFeedIndex++;

                    DebounceViewStateChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Feed rotation failed.");
                }
                finally
                {
                    Monitor.Exit(_rotateLock);
                }
            }
        }
    }
}
