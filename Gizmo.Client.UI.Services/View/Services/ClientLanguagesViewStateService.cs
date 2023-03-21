using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gizmo.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using System.Globalization;
using Gizmo.UI;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ClientLanguagesViewStateService : ViewStateServiceBase<ClientLanguagesViewState>
    {
        #region CONSTRUCTOR
        public ClientLanguagesViewStateService(ClientLanguagesViewState viewState,
            ILocalizationService localizationService,
            ICultureService cultureService,
            ILogger<ClientLanguagesViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
            _cultureService = cultureService;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly ICultureService _cultureService;
        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            var supportedCultures = _localizationService.SupportedCultures.ToList();

            var supportedCulturesViewStates = supportedCultures.Select(culture =>
            {
                return GetRequiredViewState<LanguageViewState>((state) =>
                {
                    state.NativeName = culture.NativeName;
                    state.TwoLetterName = culture.TwoLetterISOLanguageName;
                    state.LCID = culture.LCID;
                });
            }).ToList();

            ViewState.Languages = supportedCulturesViewStates;
            ViewState.SelectedLanguage = supportedCulturesViewStates.FirstOrDefault();

            return base.OnInitializing(ct);
        }

        public async void SetCurrentLanguageAsync(string twoLetterRegionName)
        {
            var language = ViewState.Languages.Where(a => a.TwoLetterName == twoLetterRegionName).FirstOrDefault();
            if (language != null)
            {
                ViewState.SelectedLanguage = language;

                var languageCulture = CultureInfo.GetCultureInfo(language.LCID);
                await  _cultureService.SetCurrentUICultureAsync(languageCulture);
                var currentLocation = NavigationService.GetUri();
                NavigationService.NavigateTo(currentLocation, new Microsoft.AspNetCore.Components.NavigationOptions() { ReplaceHistoryEntry = true, ForceLoad = false });
            }
            else
            {
                Logger.LogError("Invalid language id {languageId} specified.", twoLetterRegionName);
            }
        }
    }
}
