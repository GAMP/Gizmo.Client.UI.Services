using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class DepositTransactionViewState : ViewStateBase
    {
        #region FIELDS
        private decimal _amount;
        #endregion

        #region PROPERTIES

        public decimal Amount
        {
            get { return _amount; }
            internal set { _amount = value; }
        }

        #endregion
    }
}
