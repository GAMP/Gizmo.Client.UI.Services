using System.Globalization;

using Gizmo.UI.Services;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Web client localization service.
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

        // TODO: FOR EXAMPLE ONLY, REMOVE THIS
        public override ValueTask<IEnumerable<CultureInfo>> GetSupportedCulturesAsync()
        {
            var supportedCultures = new List<CultureInfo>()
            {
                new CultureInfo("en-US"),
                new CultureInfo("el-GR"),
                new CultureInfo("ru-RU")
            };

            SetCurrencyOptions(supportedCultures);

            return new(supportedCultures);
        }

        public override Task SetCurrentCultureAsync(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            return Task.CompletedTask;
        }
    }
}
