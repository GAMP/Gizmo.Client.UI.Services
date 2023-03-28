using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gizmo.UI.View.Services;
using Gizmo.Client.UI.View.States;
using System.Globalization;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class InputLanguagesViewStateService : ViewStateServiceBase<InputLanguagesViewState>
    {
        #region CONSTRUCTOR
        public InputLanguagesViewStateService(InputLanguagesViewState viewState,
            IInputLanguageService inputLanguageService,
            ILogger<InputLanguagesViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _inputLanguageService = inputLanguageService;
        }
        #endregion

        #region FIELDS
        private readonly IInputLanguageService _inputLanguageService;
        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            var inputLanguages = _inputLanguageService.AvailableInputLanguages;
            var currentLanguage = _inputLanguageService.CurrentInputLanguage;
            var inputLanguagesViewStates = inputLanguages.Select(culture =>
            {
                return GetRequiredViewState<LanguageViewState>((state) => state.Culture = culture);
            }).ToList();

            ViewState.Languages = inputLanguagesViewStates;
            ViewState.SelectedLanguage = GetLanguageViewState(inputLanguagesViewStates, currentLanguage.TwoLetterISOLanguageName);

            return base.OnInitializing(ct);
        }

        public async Task SetCurrentRegionAsync(string twoLetterRegionName)
        {
                ViewState.SelectedLanguage = GetLanguageViewState(ViewState.Languages, twoLetterRegionName);
                await _inputLanguageService.SetCurrentLanguageAsync(ViewState.SelectedLanguage.Culture).ConfigureAwait(true);
                ViewState.RaiseChanged();
        }

        public LanguageViewState GetLanguageViewState(IEnumerable<LanguageViewState> languages, string? twoLetterISOLanguageName) =>
            languages.FirstOrDefault(x => x.Culture.TwoLetterISOLanguageName == twoLetterISOLanguageName)
            ?? GetRequiredViewState<LanguageViewState>((state) => state.Culture = CultureInfo.CurrentCulture);
    }
}
