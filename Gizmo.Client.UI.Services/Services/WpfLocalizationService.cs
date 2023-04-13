using System.Globalization;

using Gizmo.UI;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// WPF client localization service.
    /// </summary>
    public sealed class WpfLocalizationService : ClientLocalizationServiceBase
    {
        public WpfLocalizationService(
            ILogger<WpfLocalizationService> logger,
            IStringLocalizer localizer,
            IOptions<ClientCurrencyOptions> options) : base(logger, localizer, options) { }

        public override async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            await DispatcherHelper.InvokeAsync(new Action(() =>
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }));
        }
    }
}
