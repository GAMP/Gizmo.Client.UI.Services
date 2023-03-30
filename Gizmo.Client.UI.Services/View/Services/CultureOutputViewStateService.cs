using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class CultureOutputViewStateService : ViewStateServiceBase<CultureOutputViewState>
    {
        #region CONSTRUCTOR
        public CultureOutputViewStateService(
            CultureOutputViewState viewState,
            ICultureOutputService cultureService,
            IOptions<ClientUIOptions> cultureOptions,
            ILogger<CultureOutputViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _cultureService = cultureService;
            _cultureOptions = cultureOptions.Value.CultureOutputOptions;
        }
        #endregion

        #region FIELDS
        private readonly ICultureOutputService _cultureService;
        private readonly CultureOutputOptions _cultureOptions;
        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.AvailableCultures = _cultureService.AvailableCultures.ToList();

            OverrideCulturesConfiguration();

            ViewState.CurrentCulture = _cultureService.GetCulture(ViewState.AvailableCultures,"en");

            await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(ct);
        }

        public async void SetCurrentCultureAsync(string twoLetterISOLanguageName)
        {
            ViewState.CurrentCulture = _cultureService.GetCulture(ViewState.AvailableCultures, twoLetterISOLanguageName);

            await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }

        private void OverrideCulturesConfiguration()
        {
            if (!string.IsNullOrWhiteSpace(_cultureOptions.CurrencySymbol))
            {
                foreach (var culture in ViewState.AvailableCultures)
                {
                    culture.NumberFormat.CurrencySymbol = _cultureOptions.CurrencySymbol;
                }
            }
        }
    }
}
