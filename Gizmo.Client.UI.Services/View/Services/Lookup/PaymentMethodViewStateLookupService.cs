using System.Reflection;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class PaymentMethodViewStateLookupService : ViewStateLookupServiceBase<int, PaymentMethodViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public PaymentMethodViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<PaymentMethodViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
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

            result.Name = model.Name;
            result.IsOnline = model.IsOnline;

            return result;
        }
        #endregion
    }
}
