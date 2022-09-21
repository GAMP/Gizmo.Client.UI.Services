using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PaymentMethodsViewState : ViewStateBase
    {
        #region FIELDS
        private List<PaymentMethodViewState> _paymentMethods = new();
        #endregion

        #region PROPERTIES

        public List<PaymentMethodViewState> PaymentMethods
        {
            get { return _paymentMethods; }
            internal set { _paymentMethods = value; }
        }

        #endregion
    }
}
