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
        public async ValueTask<Stream> ImageStreamGetAsync(ImageType imageType, int imageId, CancellationToken cancellationToken)
        {
            try
            {
                var imageIdHash = ImageIdHashGet(imageType, imageId);

                if (!TryGetImageFromCache(imageIdHash, out var imageData))
                {
                    imageData = await ImageGetAsync(imageType, imageId, cancellationToken);

                    var isAdded = TryAddImageToCache(imageIdHash, imageData);

                    if (!isAdded)
                    {
                        _logger.LogError("Failed adding image to cache.");
                    }
                }

                if (imageData == null)
                    return Stream.Null;

                //create recyclable stream from cached or newly obtain image buffer
                return _memoryStreamManager.GetStream(imageData);
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
        public async Task<string> ImageSourceGetAsync(ImageType imageType, int imageId, CancellationToken cToken)
        {
            var imageIdHash = ImageIdHashGet(imageType, imageId);

            if (!TryGetImageFromCache(imageIdHash, out var imageData))
            {
                imageData = await ImageGetAsync(imageType, imageId, cToken);

                var isAdded = TryAddImageToCache(imageIdHash, imageData);

                if (!isAdded)
                {
                    _logger.LogError("Failed adding image to cache.");
                }
            }

            if (imageData == null)
                return string.Empty;

            return $"data:image/png;base64,{Convert.ToBase64String(imageData)}";
        }

        private bool TryAddImageToCache(string hash, byte[] imageData)
        {
            var cache = ImageCacheGet();
            return cache.TryAdd(hash, imageData);
        }
        private bool TryGetImageFromCache(string hash, out byte[]? imageData)
        {
            var cache = ImageCacheGet();
            return cache.TryGetValue(hash, out imageData);
        }

        private ConcurrentDictionary<string, byte[]> ImageCacheGet() =>
            IsWebBrowser ? _webImageCache : _fileImageCache;
        private static string ImageIdHashGet(ImageType imageType, int imageId)
        {
            var buffer = Encoding.UTF8.GetBytes($"{imageType}{imageId}");

            return ImageHashGet(buffer);
        }
        private static string ImageHashGet(byte[] imageData)
        {
            using SHA1 provider = SHA1.Create();

            var imageHash = provider.ComputeHash(imageData);

            var result = new StringBuilder(imageHash.Length * 2);

            for (int i = 0; i < imageHash.Length; i++)
                result.Append(imageHash[i].ToString("x2"));

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
        private async ValueTask<byte[]> ImageGetAsync(ImageType imageType, int imageId, CancellationToken cToken = default)
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
    }
}
