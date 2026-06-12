using System.Globalization;
using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ClientLocalizationViewService : ViewStateServiceBase<ClientLocalizationViewState>
    {
        #region CONSTRUCTOR
        public ClientLocalizationViewService(
            ClientLocalizationViewState viewState,
            NavigationService navigationService,
            IOptionsMonitor<ClientInterfaceOptions> clientInterfaceOptions,
            ILocalizationService localizationService,
            IServerInfoService serverInfo,
            IJSRuntime jsRuntime,
            ILogger<ClientLocalizationViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _navigationService = navigationService;
            _clientInterfaceOptions = clientInterfaceOptions;
            _serverInfo = serverInfo;
            _jsRuntime = jsRuntime;
            _localizationService.LocalizationOptionsChanged += OnLocalizationOptionsChanged;
            _logger = logger;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly ILogger<ClientLocalizationViewService> _logger;
        private readonly IOptionsMonitor<ClientInterfaceOptions> _clientInterfaceOptions;
        private readonly NavigationService _navigationService;
        private readonly IServerInfoService _serverInfo;
        private readonly IJSRuntime _jsRuntime;
        private const string CultureStorageKey = "Gizmo.Client.UI.Culture";

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken cToken)
        {
            var availableCultures = (await _localizationService.GetSupportedCulturesAsync(cToken)).ToList();
            ViewState.AvailableCultures = availableCultures;

            var savedLanguage = await GetSavedCultureNameAsync(cToken);
            var preferredLanguage = _clientInterfaceOptions.CurrentValue.PreferredLanguage;

            string? serverCulture = null;
            if (string.IsNullOrWhiteSpace(savedLanguage) && string.IsNullOrWhiteSpace(preferredLanguage))
            {
                serverCulture = await GetServerDefaultCultureAsync(cToken);
            }

            ViewState.CurrentCulture =
                ResolveCulture(availableCultures, savedLanguage, preferredLanguage, serverCulture, "en-US", "en")
                ?? GetFallbackCulture(availableCultures);

            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(cToken);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            if (isDisposing)
            {
                _localizationService.LocalizationOptionsChanged -= OnLocalizationOptionsChanged;
            }

            base.OnDisposing(isDisposing);
        }

        #endregion

        #region PUBLIC FUNCTIONS

        public async Task SetCurrentCultureAsync(string cultureName)
        {
            var availableCultures = ViewState.AvailableCultures.ToList();
            var culture = ResolveCulture(availableCultures, cultureName, "en-US", "en") ?? availableCultures.FirstOrDefault();

            if (culture is null)
            {
                _logger.LogWarning("Culture '{cultureName}' was not found and no fallback culture is available.", cultureName);
                return;
            }

            ViewState.CurrentCulture = culture;

            await PersistCultureNameAsync(ViewState.CurrentCulture.Name);
            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();

            //TODO need to find a better way to do this
            var currentUri = _navigationService.GetUri();
            _navigationService.NavigateTo(currentUri ?? "/", new Microsoft.AspNetCore.Components.NavigationOptions() { ForceLoad = true });
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private static CultureInfo? ResolveCulture(IEnumerable<CultureInfo> availableCultures, params string?[] cultureNames)
        {
            var cultures = availableCultures.ToList();
            foreach (var cultureName in cultureNames)
            {
                if (string.IsNullOrWhiteSpace(cultureName))
                    continue;

                var exactMatch = cultures.FirstOrDefault(x => string.Equals(x.Name, cultureName, StringComparison.OrdinalIgnoreCase));
                if (exactMatch is not null)
                    return exactMatch;

                var neutralName = GetNeutralCultureName(cultureName);
                if (string.IsNullOrWhiteSpace(neutralName))
                    continue;

                var neutralMatch = cultures.FirstOrDefault(x => string.Equals(x.TwoLetterISOLanguageName, neutralName, StringComparison.OrdinalIgnoreCase));
                if (neutralMatch is not null)
                    return neutralMatch;
            }

            return null;
        }

        private async void OnLocalizationOptionsChanged(object? _, EventArgs __)
        {
            var availableCultures = (await _localizationService.GetSupportedCulturesAsync(default)).ToList();
            ViewState.AvailableCultures = availableCultures;
            ViewState.CurrentCulture =
                ResolveCulture(availableCultures, ViewState.CurrentCulture?.Name, "en-US", "en")
                ?? GetFallbackCulture(availableCultures);
            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }

        private async Task<string?> GetSavedCultureNameAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, CultureStorageKey);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read saved UI culture from browser storage.");
                return null;
            }
        }

        private async Task<string?> GetServerDefaultCultureAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _serverInfo.GetDefaultCultureAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load default culture from server.");
                return null;
            }
        }

        private async Task PersistCultureNameAsync(string cultureName)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CultureStorageKey, cultureName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist UI culture to browser storage.");
            }
        }

        private static string? GetNeutralCultureName(string cultureName)
        {
            try
            {
                return CultureInfo.GetCultureInfo(cultureName).TwoLetterISOLanguageName;
            }
            catch (CultureNotFoundException)
            {
                var separatorIndex = cultureName.IndexOf('-');
                return separatorIndex > 0 ? cultureName[..separatorIndex] : cultureName;
            }
        }

        private static CultureInfo GetFallbackCulture(IEnumerable<CultureInfo> availableCultures) =>
            availableCultures.FirstOrDefault() ?? CultureInfo.GetCultureInfo("en-US");

        #endregion
    }
}
