using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

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

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserPaymentMethodsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in clientResult.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.Name = item.Name;

                AddOrUpdateViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<PaymentMethodViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPaymentMethodGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (clientResult is null)
                return viewState;

            viewState.Name = clientResult.Name;

            return viewState;
        }
        protected override PaymentMethodViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<PaymentMethodViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
    }
}
