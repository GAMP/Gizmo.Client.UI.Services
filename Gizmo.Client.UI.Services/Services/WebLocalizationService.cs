using System.Globalization;
using Gizmo.Client.Options;
using Gizmo.UI;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Web client localization service.
    /// </summary>
    public class WebLocalizationService : ClientLocalizationServiceBase
    {
        #region CONSTRUCTOR
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="localizer">Localizer.</param>
        public WebLocalizationService(
            ILogger<WebLocalizationService> logger,
            IStringLocalizer localizer,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService,
            IOptionsMonitor<CurrencyOptions> options,
            IOptionsMonitor<ClientInterfaceOptions> interfaceOptions) : base(logger, localizer, assemblyResourcesLocalizationService, options, interfaceOptions) { }
        #endregion

        public override Task SetCurrentCultureAsync(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return base.SetCurrentCultureAsync(culture);
        }
    }
}
