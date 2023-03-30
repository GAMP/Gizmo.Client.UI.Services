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

        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.AvailableCultures = _localizationService.SupportedCultures;
            ViewState.CurrentCulture = _localizationService.GetCulture("en");

            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(ct);
        }

        public async void SetCurrentCultureAsync(string twoLetterISOLanguageName)
        {
            ViewState.CurrentCulture = _localizationService.GetCulture(twoLetterISOLanguageName);

            await _localizationService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }
    }
}
