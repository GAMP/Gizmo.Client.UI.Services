using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class CultureClientViewStateService : ViewStateServiceBase<CultureClientViewState>
    {
        #region CONSTRUCTOR
        public CultureClientViewStateService(
            CultureClientViewState viewState,
            ICultureService cultureService,
            IOptions<ClientUIOptions> clientOptions,
            ILogger<CultureClientViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _cultureService = cultureService;
            _clientCultureOptions = clientOptions.Value.CultureOptions;
        }
        #endregion

        #region FIELDS
        private readonly ICultureService _cultureService;
        private readonly ClientUICultureOptions _clientCultureOptions;
        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            ViewState.AveliableCultures = _cultureService.AveliableClientCultures;

            OverrideCulturesConfiguration();

            ViewState.CurrentCulture = _cultureService.GetCurrentUICulture(ViewState.AveliableCultures, "en");

            await _cultureService.SetCurrentUICultureAsync(ViewState.CurrentCulture);

            await base.OnInitializing(ct);
        }

        public async void SetCurrentCultureAsync(string twoLetterISOLanguageName)
        {
            ViewState.CurrentCulture = _cultureService.GetCurrentUICulture(ViewState.AveliableCultures, twoLetterISOLanguageName);

            await _cultureService.SetCurrentUICultureAsync(ViewState.CurrentCulture);

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
