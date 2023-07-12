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
            IOptionsMonitor<CurrencyOptions> options,
            IOptionsMonitor<ClientInterfaceOptions> interfaceOptions) : base(logger, localizer, options, interfaceOptions) { }

        public override async Task SetCurrentCultureAsync(CultureInfo culture)
        {
            //not sure why but culture changes needs to be made on both dispatcher
            //and calling thread, if not done then in some cases localization service will use previous culture

            await DispatcherHelper.InvokeAsync(new Action(() =>
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }));

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await base.SetCurrentCultureAsync(culture);
        }

        public override string GetString(string key, params object[] arguments)
        {
            return DispatcherHelper.Invoke(()=>
            { 
                return base.GetString(key, arguments);
            })!;
        }
    }
}
