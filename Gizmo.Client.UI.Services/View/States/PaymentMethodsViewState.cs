using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PaymentMethodsViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<PaymentMethodViewState> _paymentMethods = Enumerable.Empty<PaymentMethodViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<PaymentMethodViewState> PaymentMethods
        {
            get { return _paymentMethods; }
            internal set { _paymentMethods = value; }
        }

        #endregion
    }
}
