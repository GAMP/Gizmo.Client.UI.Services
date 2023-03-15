using Gizmo.UI;
using Gizmo.UI.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Image service.
    /// </summary>
    public sealed class ImageService : IImageService
    {
        #region CONSTRUCTOR
        public ImageService(ILogger<ImageService> logger, NavigationService navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;
        }
        #endregion

        #region FIELDS
        private readonly ILogger _logger;
        private readonly NavigationService _navigationManager;
        private static readonly bool _isWebBrowser = RuntimeInformation.IsOSPlatform(OSPlatform.Create("browser"));
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(100);
        private readonly RecyclableMemoryStreamManager _memoryStreamManager = new();
        #endregion

        /// <summary>
        /// Gets image stream.
        /// </summary>
        /// <param name="imageType">Image type.</param>
        /// <param name="imageId">Image id.</param>
        /// <returns>Image stream.</returns>
        public async ValueTask<Stream> ImageStreamGetAsync(ImageType imageType, int imageId, CancellationToken cancellationToken)
        {
            try
            {
                byte[] imageData = Array.Empty<byte>();
                var cacheResult = await TryGetCachedImage(imageType, imageId, cancellationToken);
                if (cacheResult.Item1 && cacheResult.Item2 != null)
                {
                    imageData = cacheResult.Item2;
                    await Task.Yield();
                }
                else
                {
        
                    imageData = await ImageGetAsync(imageType, imageId, cancellationToken);
                    await AddOrUpdateCache(imageType, imageId, imageData, cancellationToken);
                }

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

        private async ValueTask<byte[]> ImageGetAsync(ImageType imageType, int imageId, CancellationToken cancellationToken = default, bool ignoreCache = false)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                byte[] image = Array.Empty<byte>();

                using (var httpClient = new HttpClient())
                {
                    string url = string.Empty;

                    if (_isWebBrowser)
                    {
                        if (imageType == ImageType.Application)
                            url = _navigationManager.GetBaseUri() + @"_content/Gizmo.Client.UI/img/Apex.png";
                        else if (imageType == ImageType.ProductDefault)
                            url = _navigationManager.GetBaseUri() + @"_content/Gizmo.Client.UI/img/Cola2.png";
                        else
                            url = _navigationManager.GetBaseUri() + @"_content/Gizmo.Client.UI/img/Chrome-icon_1.png";
                    }
                    else
                    {
                        if (imageType == ImageType.ProductDefault)
                            url = "https://api.lorem.space/image/burger?w=200&h=300";
                        else if (imageType == ImageType.Application)
                            url = "https://api.lorem.space/image/game?w=200&h=300";
                        else
                        {
                            url = $"https://www.iconfinder.com/icons/87865/download/png/64";
                        }
                    }


                    var request = new HttpRequestMessage(HttpMethod.Get, url);

                    request.SetBrowserRequestMode(BrowserRequestMode.NoCors);

                    var response = await httpClient.SendAsync(request);
                    image = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                }

                return image;
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

        public async Task<string> ImageHashGetAsync(ImageType imageType, int imageId, CancellationToken cancellationToken)
        {
            var data = await ImageGetAsync(imageType, imageId, cancellationToken);

            using (SHA1 provider = SHA1.Create())
            {
                var hashData = provider.ComputeHash(data);
                StringBuilder result = new StringBuilder(hashData.Length * 2);

                for (int i = 0; i < hashData.Length; i++)
                    result.Append(hashData[i].ToString("x2"));

                return result.ToString();
            }
        }

        private ConcurrentDictionary<int, byte[]> _imageCache = new();
        private ConcurrentDictionary<int, byte[]> _exeImageCache = new();
        private ConcurrentDictionary<int, byte[]> _productImageCache = new();

        private ValueTask<Tuple<bool, byte[]?>> TryGetCachedImage(ImageType imageType, int imageId, CancellationToken cancellationToken = default)
        {
            var cache = GetCache(imageType);

            if (cache.TryGetValue(imageId, out var image))
                return ValueTask.FromResult(new Tuple<bool, byte[]?>(true, image));

            return ValueTask.FromResult(new Tuple<bool, byte[]?>(false, null));
        }

        private ValueTask AddOrUpdateCache(ImageType imageType, int imageId, byte[] image, CancellationToken cancellationToken = default)
        {
            var cache = GetCache(imageType);

            cache.TryAdd(imageId, image);
            return new ValueTask();
        }

        private IDictionary<int, byte[]> GetCache(ImageType imageType)
        {
            if (imageType == ImageType.Application)
                return _imageCache;
            else if(imageType == ImageType.Executable) 
                return _exeImageCache;

            return _productImageCache;

        }
    }
}
