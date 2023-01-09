using Gizmo.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Image service.
    /// </summary>
    public sealed class ImageService
    {
        #region CONSTRUCTOR
        public ImageService(ILogger<ImageService> logger, NavigationManager navigationManager)
        {
            _logger = logger;
            _navigationManager = navigationManager;
        }
        #endregion

        #region FIELDS
        private readonly ILogger _logger;
        private readonly NavigationManager _navigationManager;
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
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                //first we need to obtain the image hash from server
                var hash = await ImageHashGetAsync(imageType, imageId, cancellationToken);

                //if hash obtained successfully we need to check if locally cached entry exist

                bool cachedEntry = false;

                byte[] imageData = Array.Empty<byte>();

                if (!cachedEntry)
                {
                    //if we failed to obtain image data from cache we need to call resepctive backend to obtain it
                    //get image data
                    imageData = await ImageGetAsync(imageType, imageId, cancellationToken);

                    //once the image data is obtained we need to store it in local cache
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
            finally
            {
                _semaphore.Release();
            }
        }

        private async ValueTask<byte[]> ImageGetAsync(ImageType imageType, int imageId, CancellationToken cancellationToken =default, bool ignoreCache= false)
        {
            byte[] image;

            if (!_isWebBrowser)
            {
                //obtain image from source or memorty cache
                image = await File.ReadAllBytesAsync("C:\\Users\\Dabuzz\\Desktop\\modern-warfare-2-cover-art.webp", cancellationToken);
            }
            else
            {
                using (var httpClient = new HttpClient())
                {
                    string url = string.Empty;

                    if (imageType == ImageType.Application)
                        url = _navigationManager.BaseUri.ToString() + @"_content/Gizmo.Client.UI/img/Apex.png";
                    else
                        url = _navigationManager.BaseUri.ToString() + @"_content/Gizmo.Client.UI/img/Cola2.png";

                    image = await httpClient.GetByteArrayAsync(url, cancellationToken);
                    //image = await httpClient.GetByteArrayAsync(@"https://sportshub.cbsistatic.com/i/2022/05/25/a9564e17-4ae5-4637-8843-045cf48979dc/modern-warfare-2-cover-art.jpg?auto=webp&width=1539&height=1920&crop=0.802:1,smart", cancellationToken);
                }
            }

            return image;
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
    }
}
