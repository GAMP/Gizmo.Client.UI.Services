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
                return GetRequiredViewState<LanguageViewState>((state) =>
                {
                    state.EnglishName = culture.EnglishName;
                    state.TwoLetterName = culture.TwoLetterISOLanguageName;
                    state.LCID = culture.LCID;
                    state.NativeName = culture.NativeName;
                });
            }).ToList();

            ViewState.Languages = inputLanguagesViewStates;
            ViewState.SelectedLanguage = ViewState.Languages
                .Where(viewState=>viewState.TwoLetterName == currentLanguage.TwoLetterISOLanguageName)
                .FirstOrDefault();

            return base.OnInitializing(ct);
        }

        public async Task SetCurrentRegionAsync(string twoLetterRegionName)
        {
            var region = ViewState.Languages
                .Where(a => a.TwoLetterName == twoLetterRegionName)
                .FirstOrDefault();

            if (region != null)
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(twoLetterRegionName);
                await _inputLanguageService.SetCurrentLanguageAsync(cultureInfo).ConfigureAwait(true);
                ViewState.SelectedLanguage = region;
                ViewState.RaiseChanged();
            }
            else
            {
                Logger.LogError("Invalid region id {regionId} specified.", twoLetterRegionName);
            }
        }
    }
}
