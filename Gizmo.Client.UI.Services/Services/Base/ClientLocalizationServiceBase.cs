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
        private readonly CultureInfo _defaultCulture = CultureInfo.CurrentCulture;

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
                else
                    culture.NumberFormat.CurrencySymbol = _defaultCulture.NumberFormat.CurrencySymbol;

                if (_currencyOptions.CurrencyDecimalDigits.HasValue)
                    culture.NumberFormat.CurrencyDecimalDigits = _currencyOptions.CurrencyDecimalDigits.Value;
                else
                    culture.NumberFormat.CurrencyDecimalDigits = _defaultCulture.NumberFormat.CurrencyDecimalDigits;

                if (!string.IsNullOrWhiteSpace(_currencyOptions.CurrencyDecimalSeparator))
                    culture.NumberFormat.CurrencyDecimalSeparator = _currencyOptions.CurrencyDecimalSeparator;
                else
                    culture.NumberFormat.CurrencyDecimalSeparator = _defaultCulture.NumberFormat.CurrencyDecimalSeparator;

                if (!string.IsNullOrWhiteSpace(_currencyOptions.CurrencyGroupSeparator))
                    culture.NumberFormat.CurrencyGroupSeparator = _currencyOptions.CurrencyGroupSeparator;
                else
                    culture.NumberFormat.CurrencyGroupSeparator = _defaultCulture.NumberFormat.CurrencyGroupSeparator;

                if (_currencyOptions.CurrencyGroupSizes != null)
                    culture.NumberFormat.CurrencyGroupSizes = _currencyOptions.CurrencyGroupSizes;
                else
                    culture.NumberFormat.CurrencyGroupSizes = _defaultCulture.NumberFormat.CurrencyGroupSizes;

                if (_currencyOptions.CurrencyNegativePattern.HasValue)
                    culture.NumberFormat.CurrencyNegativePattern = _currencyOptions.CurrencyNegativePattern.Value;
                else
                    culture.NumberFormat.CurrencyNegativePattern = _defaultCulture.NumberFormat.CurrencyNegativePattern;

                if (_currencyOptions.CurrencyPositivePattern.HasValue)
                    culture.NumberFormat.CurrencyPositivePattern = _currencyOptions.CurrencyPositivePattern.Value;
                else
                    culture.NumberFormat.CurrencyPositivePattern = _defaultCulture.NumberFormat.CurrencyPositivePattern;
            }
        }
    }
}
