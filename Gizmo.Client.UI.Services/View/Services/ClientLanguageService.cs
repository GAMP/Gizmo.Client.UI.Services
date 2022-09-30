using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Gizmo.UI.View.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ClientLanguageService : ViewStateServiceBase<ClientLanguageViewState>
    {
        #region CONSTRUCTOR
        public ClientLanguageService(ClientLanguageViewState viewState,
            ILocalizationService localizationService,
            ILogger<ClientLanguageService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
            _loicalizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _loicalizationService; 
        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            foreach (var culture in _loicalizationService.SupportedRegions)
            {
                var viewState = GetViewState<RegionViewState>((state) =>
                {
                    state.EnglishName = culture.EnglishName;
                    state.TwoLetterISORegionName = culture.TwoLetterISORegionName;
                });

                ViewState.Regions.Add(viewState);
            }

            return base.OnInitializing(ct);
        }

        public Task SetCurrentLanguageAsync(int twoLetterRegionName)
        {
            if (twoLetterRegionName <= 0)
                Logger.LogError("Invalid region id {regionId} specified.", twoLetterRegionName);

            return Task.CompletedTask;
        }
    }
}
