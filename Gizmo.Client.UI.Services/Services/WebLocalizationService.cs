using System.Globalization;
using System.Reflection;
using System.Resources;
using Gizmo.UI.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Client localization service.
    /// </summary>
    public class WebLocalizationService : LocalizationServiceBase
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
            IOptions<ClientUIOptions> options) : base(logger, localizer, options)
        {
            var prop = localizer.GetType().GetField("_localizer", BindingFlags.NonPublic | BindingFlags.Instance);
            _resourceManagerStringLocalizer = (ResourceManagerStringLocalizer)prop.GetValue(localizer);

            prop = _resourceManagerStringLocalizer.GetType().GetField("_resourceManager", BindingFlags.NonPublic | BindingFlags.Instance);
            _resourceManager = (ResourceManager)prop.GetValue(_resourceManagerStringLocalizer);
        }
        #endregion



        public override IEnumerable<CultureInfo> SupportedCultures
        {
            get
            {
                var supportedCultures = new HashSet<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ru-RU"),
                    new CultureInfo("el-GR")
                };

                if (_resourceManager is not null)
                {
                    supportedCultures = supportedCultures.Where(culture =>
                    {
                        try
                        {
                            var resourceSet = _resourceManager?.GetResourceSet(culture, true, false);
                            return resourceSet != null;
                        }
                        catch (CultureNotFoundException ex)
                        {
                            Logger.LogError(ex, "Could not obtain resource set for {culture}.", culture);
                            return false;
                        }
                    }).ToHashSet();
                }

                OverrideCultureCurrencyConfiguration(supportedCultures);

                return supportedCultures;
            }
        }

        public override Task SetCurrentCultureAsync(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return Task.CompletedTask;
        }

        public override CultureInfo GetCulture(string twoLetterISOLanguageName)
        {
            return SupportedCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
           ?? CultureInfo.CurrentUICulture;
        }
    }
}
