using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Image service.
    /// </summary>
    public sealed class ImageService : IImageService
    {
        private static readonly bool IsWebBrowser = RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"));

        #region CONSTRUCTOR
        public ImageService(ILogger<ImageService> logger, IHttpClientFactory httpClientFactory, NavigationService navigationManager)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _navigationManager = navigationManager;
        }
        #endregion

        #region PRIVATE FIELDS
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationService _navigationManager;
        private readonly SemaphoreSlim _semaphore = new(100);
        private readonly RecyclableMemoryStreamManager _memoryStreamManager = new();
        #endregion

        #region PUBLIC FUNCTIONS
        /// <inheritdoc/>
        public async ValueTask<Stream> ImageStreamGetAsync(ImageType imageType, int imageId, CancellationToken cToken)
        {
            try
            {
                var imageData = await ImageCachedDataGetAsync(imageType, imageId, cToken);

                return imageData is null
                    ? Stream.Null
                    : _memoryStreamManager.GetStream(imageData); // create recyclable stream
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed obtaining image stream.");
                //in case of error return empty stream
                return Stream.Null;
            }
        }
        /// <inheritdoc/>
        public async ValueTask<string> ImageSourceGetAsync(ImageType imageType, int imageId, CancellationToken cToken)
        {
            var imageData = await ImageCachedDataGetAsync(imageType, imageId, cToken);

            return imageData is null
                ? string.Empty
                : $"data:image/png;base64,{Convert.ToBase64String(imageData)}";
        }
        #endregion

        #region IMAGE CACHE STORAGES 
        private readonly ConcurrentDictionary<string, byte[]> _webImageCache = new();
        private readonly ConcurrentDictionary<string, byte[]> _fileImageCache = new();

        private IDictionary<string, byte[]> WebCache() => _webImageCache;
        private IDictionary<string, byte[]> FileCache() => _fileImageCache;
        #endregion

        #region IMAGE CACHE FUNCTIONS
        private IDictionary<string, byte[]> ImageCache() => IsWebBrowser ? WebCache() : FileCache();

        private bool TryAddImageToCache(string hash, byte[] imageData) =>
            ImageCache().TryAdd(hash, imageData);
        private bool TryGetImageFromCache(string hash, out byte[]? imageData) =>
            ImageCache().TryGetValue(hash, out imageData);
        #endregion

        #region IMAGE DATA FUNCTIONS
        private static string SHA1HashCompute(byte[] data)
        {
            using SHA1 provider = SHA1.Create();

            var hash = provider.ComputeHash(data);

            var result = new StringBuilder(hash.Length * 2);

            for (int i = 0; i < hash.Length; i++)
                result.Append(hash[i].ToString("x2"));

            return result.ToString();
        }
        private string ImageUrlGet(ImageType imageType) => IsWebBrowser
            ? _navigationManager.GetBaseUri() + imageType switch
            {
                ImageType.Application => "_content/Gizmo.Client.UI/img/DemoApex.png",
                ImageType.ProductDefault => "_content/Gizmo.Client.UI/img/DemoCola2.png",
                _ => "_content/Gizmo.Client.UI/img/DemoChrome-icon_1.png"
            }
            : imageType switch
            {
                ImageType.ProductDefault => "https://api.lorem.space/image/burger?w=200&h=300",
                ImageType.Application => "https://api.lorem.space/image/game?w=200&h=300",
                _ => "https://www.iconfinder.com/icons/87865/download/png/64"
            };
        private async ValueTask<byte[]> ImageServerDataGetAsync(ImageType imageType, int imageId, CancellationToken cToken)
        {
            await _semaphore.WaitAsync(cToken);

            var httpClient = _httpClientFactory.CreateClient(nameof(ImageService));

            var imageUrl = ImageUrlGet(imageType);

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, imageUrl);

                request.SetBrowserRequestMode(BrowserRequestMode.NoCors);

                var response = await httpClient.SendAsync(request, cToken);

                return await response.Content.ReadAsByteArrayAsync(cToken);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private async ValueTask<byte[]?> ImageCachedDataGetAsync(ImageType imageType, int imageId, CancellationToken cToken)
        {
            var buffer = Encoding.UTF8.GetBytes($"{imageType}{imageId}");

            var hash = SHA1HashCompute(buffer);

            if (!TryGetImageFromCache(hash, out var data))
            {
                data = await ImageServerDataGetAsync(imageType, imageId, cToken);

                var isCached = TryAddImageToCache(hash, data);

                if (!isCached)
                {
                    _logger.LogError("Failed adding image to cache.");
                }
            }

            return data;
        }
        #endregion
    }
}
