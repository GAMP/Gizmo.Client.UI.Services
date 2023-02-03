using Microsoft.Extensions.Logging;
using System.Reflection;
using Gizmo.UI.Services;
using Microsoft.Extensions.Options;
using Gizmo.UI;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// UI Composition service used in web assembly host.
    /// </summary>
    public sealed class WebAssemblyUICompositionService : UICompositionServiceBase
    {
        #region CONSTRUCTOR
        public WebAssemblyUICompositionService(IOptionsMonitor<UICompositionOptions> optionsMonitor,
            IServiceProvider serviceProvider,
            IHttpClientFactory httpClientFactory,
            ILogger<WebAssemblyUICompositionService> logger) : base(optionsMonitor, logger, serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
        }
        #endregion

        #region FIELDS
        private readonly IHttpClientFactory _httpClientFactory;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets http client factory.
        /// </summary>
        private IHttpClientFactory HttpClientFactory => _httpClientFactory;

        #endregion

        #region OVERRIDES

        protected async override Task<Assembly> LoadAssemblyAsync(string assemblyName, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName));

            using (HttpClient client = HttpClientFactory.CreateClient("Default"))
            {
                //the default client should have the base address set so we can start requesting the assembly data with relative path

                //try to fetch external assembly
                var externallib = await client.GetByteArrayAsync($"/_framework/{assemblyName}", ct).ConfigureAwait(false);

                //load the external assembly into app domain
                var assembly = AppDomain.CurrentDomain.Load(externallib);

                //return loaded assembly
                return assembly;
            }
        }

        #endregion
    }
}