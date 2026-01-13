using System.Globalization;
using Gizmo.Client.Options;
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
        private readonly IOptionsMonitor<ClientInterfaceOptions> _interfaceOptions;
        private readonly CultureInfo _defaultCulture = CultureInfo.CurrentCulture;

        protected ClientLocalizationServiceBase(ILogger logger,
            IStringLocalizer localizer,
            IAssemblyResourcesLocalizationService assemblyResourcesLocalizationService,
            IOptionsMonitor<CurrencyOptions> options,
            IOptionsMonitor<ClientInterfaceOptions> interfaceOptions) : base(logger, localizer)
        {
            _assemblyResourcesLocalizationService = assemblyResourcesLocalizationService;
            _interfaceOptions = interfaceOptions;
            _currencyOptions = options.CurrentValue;

            _interfaceOptions.OnChange(OnInterfaceOptionsChange);

            options.OnChange(currencyOptions =>
            {
                _currencyOptions = currencyOptions;
                LocalizationOptionsChanged?.Invoke(this, EventArgs.Empty);
            });
        }

        private readonly IAssemblyResourcesLocalizationService _assemblyResourcesLocalizationService;

        public override event EventHandler<EventArgs>? LocalizationOptionsChanged;

        public override event EventHandler<EventArgs>? LanguageChanged;

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

        public override Task SetCurrentCultureAsync(CultureInfo culture)
        {
            _assemblyResourcesLocalizationService.SetCulture(culture);
            LanguageChanged?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        private void OnInterfaceOptionsChange(ClientInterfaceOptions clientInterfaceOptions)
        {
            if (!string.IsNullOrWhiteSpace(clientInterfaceOptions.PreferredLanguage))
            {
                var preferredCulture = CultureInfo.CreateSpecificCulture(clientInterfaceOptions.PreferredLanguage);
                if (preferredCulture != null)
                {
                    SetCurrentCultureAsync(preferredCulture)
                        .ContinueWith((t) =>
                        {
                            Logger.LogError(t.Exception, "Failed to set preferred culture on client interface options change.");
                        }, TaskContinuationOptions.OnlyOnFaulted)
                        .ConfigureAwait(false);
                }
                else
                {
                    Logger.LogWarning("Invalid preferred culture [{culture}] found in client interface options.", clientInterfaceOptions.PreferredLanguage);
                }
            }
        }
    }
}
