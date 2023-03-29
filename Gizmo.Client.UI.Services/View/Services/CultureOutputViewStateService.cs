using Gizmo.Client.UI.Services;
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
            CultureOutputService cultureService,
            IOptions<ClientUIOptions> clientOptions,
            ILogger<CultureOutputViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _cultureService = cultureService;
            _clientCultureOptions = clientOptions.Value.CultureOptions;
        }
        #endregion

        #region FIELDS
        private readonly CultureOutputService _cultureService;
        private readonly ClientUICultureOptions _clientCultureOptions;
        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.AveliableCultures = _cultureService.AveliableCultures;

            OverrideCulturesConfiguration();

            ViewState.CurrentCulture = _cultureService.GetCurrentCulture("ru");

            await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(ct);
        }

        public async void SetCurrentCultureAsync(string twoLetterISOLanguageName)
        {
            ViewState.CurrentCulture = _cultureService.GetCurrentCulture(twoLetterISOLanguageName);

            await _cultureService.SetCurrentCultureAsync(ViewState.CurrentCulture);

            ViewState.RaiseChanged();
        }

        private void OverrideCulturesConfiguration()
        {
            if (!string.IsNullOrWhiteSpace(_clientCultureOptions.CurrencySymbol))
            {
                foreach (var culture in ViewState.AveliableCultures)
                {
                    culture.NumberFormat.CurrencySymbol = _clientCultureOptions.CurrencySymbol;
                }
            }
        }
    }
}
