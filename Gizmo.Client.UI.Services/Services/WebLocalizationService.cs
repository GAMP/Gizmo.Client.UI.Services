using System.Globalization;

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
            IOptions<ClientCurrencyOptions> options) : base(logger, localizer, options) { }
        #endregion

        public override IEnumerable<CultureInfo> GetSupportedCultures()
        {
            var supportedCultures = new List<CultureInfo>()
            {
                new CultureInfo("en-US"),
                new CultureInfo("el-GR"),
                new CultureInfo("ru-RU")
            };

            SetCurrencyOptions(supportedCultures);

            return supportedCultures;
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
