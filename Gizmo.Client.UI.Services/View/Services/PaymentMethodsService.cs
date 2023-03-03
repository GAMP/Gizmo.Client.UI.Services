using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class PaymentMethodsService : ViewStateServiceBase<PaymentMethodsViewState>
    {
        #region CONSTRUCTOR
        public PaymentMethodsService(
            ILogger<PaymentMethodsService> logger,
            IServiceProvider serviceProvider,
            PaymentMethodsViewState viewState,
            PaymentMethodViewStateLookupService paymentMethodLookupService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _paymentMethodLookupService = paymentMethodLookupService;
            _gizmoClient = gizmoClient;
        }

        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly PaymentMethodViewStateLookupService _paymentMethodLookupService;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadPaymentMethods()
        {
            //TODO: A Load payment methods on user login?

            //Test
            Random random = new Random();

            var paymentMethods = await ((TestClient)_gizmoClient).PaymentMethodsGetAsync(new PaymentMethodsFilter());
            ViewState.PaymentMethods = paymentMethods.Data.Select(a => new PaymentMethodViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
            //End Test

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
