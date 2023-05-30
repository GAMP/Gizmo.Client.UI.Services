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
        #region CONSTRUCTOR
        public ImageService(ILogger<ImageService> logger, IHttpClientFactory httpClientFactory, NavigationService navigationManager)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _navigationManager = navigationManager;
        }

        #endregion

        #region FIELDS
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationService _navigationManager;
        private static readonly bool IsWebBrowser = RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"));
        private readonly SemaphoreSlim _semaphore = new(100);
        private readonly RecyclableMemoryStreamManager _memoryStreamManager = new();

        private readonly ConcurrentDictionary<string, byte[]> _webImageCache = new();
        private readonly ConcurrentDictionary<string, byte[]> _fileImageCache = new();
        #endregion

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

        private ConcurrentDictionary<string, byte[]> ImageCacheGet() =>
            IsWebBrowser ? _webImageCache : _fileImageCache;
        private bool TryAddImageToCache(string hash, byte[] imageData) =>
            ImageCacheGet().TryAdd(hash, imageData);
        private bool TryGetImageFromCache(string hash, out byte[]? imageData) =>
            ImageCacheGet().TryGetValue(hash, out imageData);

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
    }
}
