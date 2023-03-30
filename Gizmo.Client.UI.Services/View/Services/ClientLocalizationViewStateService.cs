using System.Globalization;

using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ClientLocalizationViewStateService : ViewStateServiceBase<ClientLocalizationViewState>
    {
        #region CONSTRUCTOR
        public ClientLocalizationViewStateService(
            ClientLocalizationViewState viewState,
            ILocalizationService localizationService,
            ILogger<ClientLocalizationViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        #endregion

        #region OVERRIDES
        
        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.AvailableCultures = await _localizationService.GetSupportedCulturesAsync();
            ViewState.CurrentCulture = GetCulture(ViewState.AvailableCultures, "en");

            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(ct);
        }
        
        #endregion

        #region PUBLIC FUNCTIONS
        
        public async void SetCurrentCultureAsync(string twoLetterISOLanguageName)
        {
            ViewState.CurrentCulture = GetCulture(ViewState.AvailableCultures, twoLetterISOLanguageName);

            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }

        #endregion

        #region PRIVATE FUNCTIONS
        
        private CultureInfo GetCulture(IEnumerable<CultureInfo> cultures, string twoLetterISOLanguageName)
        {
            return cultures.FirstOrDefault(x => x.TwoLetterISOLanguageName == twoLetterISOLanguageName)
           ?? new CultureInfo("en-US");
        }
        
        #endregion
    }
}
