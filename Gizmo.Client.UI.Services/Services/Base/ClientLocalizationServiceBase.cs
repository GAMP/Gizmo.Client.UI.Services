using System.Globalization;
using Gizmo.UI.Services;

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.Services
{
    public abstract class ClientLocalizationServiceBase : LocalizationServiceBase
    {
        private readonly ClientCurrencyOptions _cultureOptions;

        protected ClientLocalizationServiceBase(ILogger logger, IStringLocalizer localizer, IOptions<ClientCurrencyOptions> options) : base(logger, localizer)
        {
            _cultureOptions = options.Value;
        }

        /// <summary>
        /// Sets currency options  from the configuration for the <paramref name="cultures"/>.
        /// </summary>
        /// <param name="cultures">
        /// Cultures to set currency options for.
        /// </param>
        protected override void ConfigureLocalizationOptions(IEnumerable<CultureInfo> cultures)
        {
            if (!string.IsNullOrWhiteSpace(_cultureOptions.CurrencySymbol))
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencySymbol = _cultureOptions.CurrencySymbol;

            if (_cultureOptions.CurrencyDecimalDigits.HasValue)
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencyDecimalDigits = _cultureOptions.CurrencyDecimalDigits.Value;

            if (!string.IsNullOrWhiteSpace(_cultureOptions.CurrencyDecimalSeparator))
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencyDecimalSeparator = _cultureOptions.CurrencyDecimalSeparator;

            if (!string.IsNullOrWhiteSpace(_cultureOptions.CurrencyGroupSeparator))
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencyGroupSeparator = _cultureOptions.CurrencyGroupSeparator;

            if (_cultureOptions.CurrencyGroupSizes != null)
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencyGroupSizes = _cultureOptions.CurrencyGroupSizes;

            if (_cultureOptions.CurrencyNegativePattern.HasValue)
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencyNegativePattern = _cultureOptions.CurrencyNegativePattern.Value;

            if (_cultureOptions.CurrencyPositivePattern.HasValue)
                foreach (var culture in cultures)
                    culture.NumberFormat.CurrencyPositivePattern = _cultureOptions.CurrencyPositivePattern.Value;
        }
    }
}
