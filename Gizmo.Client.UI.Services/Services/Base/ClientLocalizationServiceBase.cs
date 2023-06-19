using System.Globalization;

using Gizmo.UI;
using Gizmo.UI.Services;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    public abstract class ClientLocalizationServiceBase : LocalizationServiceBase
    {
        private CurrencyOptions _currencyOptions;

        protected ClientLocalizationServiceBase(ILogger logger, IStringLocalizer localizer, IOptionsMonitor<CurrencyOptions> options) : base(logger, localizer)
        {
            _currencyOptions = options.CurrentValue;

            options.OnChange(currencyOptions =>
            {
                _currencyOptions = currencyOptions;
                LocalizationOptionsChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        public override event EventHandler<EventArgs>? LocalizationOptionsChanged;

        /// <summary>
        /// Sets currency options  from the configuration for the <paramref name="cultures"/>.
        /// </summary>
        /// <param name="cultures">
        /// Cultures to set currency options for.
        /// </param>
        protected override void ConfigureLocalizationOptions(IEnumerable<CultureInfo> cultures)
        {
            foreach (var culture in cultures)
            {
                culture.NumberFormat.CurrencySymbol = _currencyOptions.CurrencySymbol ?? culture.NumberFormat.CurrencySymbol;
                culture.NumberFormat.CurrencyDecimalDigits = _currencyOptions.CurrencyDecimalDigits ?? culture.NumberFormat.CurrencyDecimalDigits;
                culture.NumberFormat.CurrencyDecimalSeparator = _currencyOptions.CurrencyDecimalSeparator ?? culture.NumberFormat.CurrencyDecimalSeparator;
                culture.NumberFormat.CurrencyGroupSeparator = _currencyOptions.CurrencyGroupSeparator ?? culture.NumberFormat.CurrencyGroupSeparator;
                culture.NumberFormat.CurrencyGroupSizes = _currencyOptions.CurrencyGroupSizes ?? culture.NumberFormat.CurrencyGroupSizes;
                culture.NumberFormat.CurrencyNegativePattern = _currencyOptions.CurrencyNegativePattern ?? culture.NumberFormat.CurrencyNegativePattern;
                culture.NumberFormat.CurrencyPositivePattern = _currencyOptions.CurrencyPositivePattern ?? culture.NumberFormat.CurrencyPositivePattern;
            }
        }
    }
}
