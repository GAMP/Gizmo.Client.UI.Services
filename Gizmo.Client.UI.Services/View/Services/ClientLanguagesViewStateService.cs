using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gizmo.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using System.Globalization;
using Gizmo.UI;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ClientLanguagesViewStateService : ViewStateServiceBase<ClientLanguagesViewState>
    {
        #region CONSTRUCTOR
        public ClientLanguagesViewStateService(
            ClientLanguagesViewState viewState,
            ILocalizationService localizationService,
            ICultureService cultureService,
            IOptions<ClientUIOptions> clientOptions,
            ILogger<ClientLanguagesViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _cultureService = cultureService;
            _localizationService = localizationService;
            _clientCultureOptions = clientOptions.Value.CultureOptions;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly ICultureService _cultureService;
        private readonly ClientUICultureOptions _clientCultureOptions;
        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            var supportedCultures = _localizationService.SupportedCultures.ToList();

            var isCustomCurrencySymbol = !string.IsNullOrWhiteSpace(_clientCultureOptions.CurrencySymbol);

            var supportedLanguages = supportedCultures.Select(culture =>
            {
                if (isCustomCurrencySymbol)
                    culture.NumberFormat.CurrencySymbol = _clientCultureOptions.CurrencySymbol!;

                return GetRequiredViewState<LanguageViewState>((state) => state.Culture = culture);

            }).ToList();

            ViewState.Languages = supportedLanguages;
            ViewState.SelectedLanguage = GetLanguageViewState(supportedLanguages, "en");

            await _cultureService.SetCurrentUICultureAsync(ViewState.SelectedLanguage.Culture);
            
            await base.OnInitializing(ct);
        }

        public async void SetCurrentLanguageAsync(string twoLetterRegionName)
        {
            var targetLanguge = GetLanguageViewState(ViewState.Languages, twoLetterRegionName);

            await _cultureService.SetCurrentUICultureAsync(targetLanguge.Culture);

            var currentLocation = NavigationService.GetUri();
            NavigationService.NavigateTo(currentLocation, new Microsoft.AspNetCore.Components.NavigationOptions() { ReplaceHistoryEntry = true, ForceLoad = false });
        }

        public LanguageViewState GetLanguageViewState(IEnumerable<LanguageViewState> languages, string? twoLetterISOLanguageName) =>
            languages.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? GetRequiredViewState<LanguageViewState>((state) => state.Culture = CultureInfo.CurrentCulture);
    }
}
