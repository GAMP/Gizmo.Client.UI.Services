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
                if (!string.IsNullOrWhiteSpace(_currencyOptions.CurrencySymbol))
                        culture.NumberFormat.CurrencySymbol = _currencyOptions.CurrencySymbol;

                if (_currencyOptions.CurrencyDecimalDigits.HasValue)
                        culture.NumberFormat.CurrencyDecimalDigits = _currencyOptions.CurrencyDecimalDigits.Value;

                if (!string.IsNullOrWhiteSpace(_currencyOptions.CurrencyDecimalSeparator))
                        culture.NumberFormat.CurrencyDecimalSeparator = _currencyOptions.CurrencyDecimalSeparator;

                if (!string.IsNullOrWhiteSpace(_currencyOptions.CurrencyGroupSeparator))
                        culture.NumberFormat.CurrencyGroupSeparator = _currencyOptions.CurrencyGroupSeparator;

                if (_currencyOptions.CurrencyGroupSizes != null)
                        culture.NumberFormat.CurrencyGroupSizes = _currencyOptions.CurrencyGroupSizes;

                if (_currencyOptions.CurrencyNegativePattern.HasValue)
                        culture.NumberFormat.CurrencyNegativePattern = _currencyOptions.CurrencyNegativePattern.Value;

                if (_currencyOptions.CurrencyPositivePattern.HasValue)
                        culture.NumberFormat.CurrencyPositivePattern = _currencyOptions.CurrencyPositivePattern.Value;
            }
        }
    }
}
