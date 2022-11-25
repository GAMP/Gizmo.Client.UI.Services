using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gizmo.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ClientLanguagesService : ViewStateServiceBase<ClientLanguagesViewState>
    {
        #region CONSTRUCTOR
        public ClientLanguagesService(ClientLanguagesViewState viewState,
            ILocalizationService localizationService,
            ILogger<ClientLanguagesService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService; 
        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            foreach (var culture in _localizationService.SupportedCultures)
            {
                var viewState = GetViewState<LanguageViewState>((state) =>
                {
                    state.NativeName = culture.NativeName;
                    state.TwoLetterName = culture.TwoLetterISOLanguageName;
                });

                ViewState.Languages.Add(viewState);
            }

            ViewState.SelectedLanguage = ViewState.Languages.FirstOrDefault();

            return base.OnInitializing(ct);
        }

        public Task SetCurrentLanguageAsync(string twoLetterRegionName)
        {
            var language = ViewState.Languages.Where(a => a.TwoLetterName == twoLetterRegionName).FirstOrDefault();
            if (language != null)
            {
                ViewState.SelectedLanguage = language;
                ViewState.RaiseChanged();
            }
            else
            {
                Logger.LogError("Invalid language id {languageId} specified.", twoLetterRegionName);
            }

            return Task.CompletedTask;
        }
    }
}
