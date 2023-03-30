using System.Globalization;

using Gizmo.UI;
using Gizmo.UI.Services;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    public sealed class WpfLocalizationService : LocalizationServiceBase
    {
        public WpfLocalizationService(
            ILogger<WpfLocalizationService> logger,
            IStringLocalizer localizer,
            IOptions<ClientCurrencyOptions> options) : base(logger, localizer, options) { }

        /// <summary>
        /// Sets current UI culture.
        /// </summary>
        /// <param name="culture">Culture.</param>
        public override async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            await DispatcherHelper.InvokeAsync(new Action(() =>
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }));
        }

        public override CultureInfo GetCulture(string twoLetterISOLanguageName) =>
            SupportedCultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? CultureInfo.CurrentCulture;
    }
}
