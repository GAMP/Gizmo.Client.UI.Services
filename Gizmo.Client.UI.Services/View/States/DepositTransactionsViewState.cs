using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class DepositTransactionsViewState : ViewStateBase
    {
        #region FIELDS
        private List<DepositTransactionViewState> _depositTransactions = new();
        #endregion

        #region PROPERTIES

        public List<DepositTransactionViewState> DepositTransactions
        {
            get { return _depositTransactions; }
            internal set { _depositTransactions = value; }
        }

        #endregion
    }
}
