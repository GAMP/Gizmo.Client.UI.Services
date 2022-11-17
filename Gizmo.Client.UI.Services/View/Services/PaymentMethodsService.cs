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
        public PaymentMethodsService(PaymentMethodsViewState viewState,
            ILogger<PaymentMethodsService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS
        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            Random random = new Random();

            var paymentMethods = await _gizmoClient.GetPaymentMethodsAsync(new PaymentMethodsFilter());
            ViewState.PaymentMethods = paymentMethods.Data.Select(a => new PaymentMethodViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();
        }
    }
}