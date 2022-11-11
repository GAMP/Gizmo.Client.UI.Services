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
            foreach (var culture in _localizationService.SupportedRegions)
            {
                var viewState = GetViewState<RegionViewState>((state) =>
                {
                    state.EnglishName = culture.EnglishName;
                    state.TwoLetterISORegionName = culture.TwoLetterISORegionName;
                });

                ViewState.Regions.Add(viewState);
            }

            ViewState.SelectedRegion = ViewState.Regions.FirstOrDefault();

            return base.OnInitializing(ct);
        }

        public Task SetCurrentRegionAsync(string twoLetterRegionName)
        {
            var region = ViewState.Regions.Where(a => a.TwoLetterISORegionName == twoLetterRegionName).FirstOrDefault();
            if (region != null)
            {
                ViewState.SelectedRegion = region;
                ViewState.RaiseChanged();
            }
            else
            {
                Logger.LogError("Invalid region id {regionId} specified.", twoLetterRegionName);
            }

            return Task.CompletedTask;
        }
    }
}
