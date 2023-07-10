using System.Reflection;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class PaymentMethodViewStateLookupService : ViewStateLookupServiceBase<int, PaymentMethodViewState>
    {
        public PaymentMethodViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<PaymentMethodViewStateLookupService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
        }

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        #endregion

        private async void OnLanguageChanged(object? sender, EventArgs e)
        {
            try
            {
                var states = await GetStatesAsync();

                foreach (var state in states.Where(a => a.Id < 0))
                {
                    switch (state.Id)
                    {
                        case -4: //Points
                            state.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_POINTS");
                            break;

                        case -3: //Deposit
                            state.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_DEPOSIT");
                            break;

                        case -2: //Credit Card
                            state.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_CREDIT_CARD");
                            break;

                        case -1: //Cash
                            state.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_CASH");
                            break;

                        default:
                            state.Name = state.Name;
                            break;
                    }
                    state.RaiseChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to translate payment methods.");
            }
        }

        #region OVERRIDED FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, PaymentMethodViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserPaymentMethodsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<PaymentMethodViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPaymentMethodGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<PaymentMethodViewState> UpdateViewStateAsync(PaymentMethodViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPaymentMethodGetAsync(viewState.Id, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override PaymentMethodViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<PaymentMethodViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default name";
            defaultState.IsOnline = false;

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private PaymentMethodViewState Map(UserPaymentMethodModel model, PaymentMethodViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            switch (model.Id)
            {
                case -4: //Points
                    result.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_POINTS");
                    break;

                case -3: //Deposit
                    result.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_DEPOSIT");
                    break;

                case -2: //Credit Card
                    result.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_CREDIT_CARD");
                    break;

                case -1: //Cash
                    result.Name = _localizationService.GetString("GIZ_GEN_PAYMENT_METHOD_CASH");
                    break;

                default:
                    result.Name = model.Name;
                    break;
            }

            result.IsOnline = model.IsOnline;

            return result;
        }
        #endregion
    }
}
