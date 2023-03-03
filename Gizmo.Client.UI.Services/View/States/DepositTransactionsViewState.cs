using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class DepositTransactionsViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<DepositTransactionViewState> _depositTransactions = Enumerable.Empty<DepositTransactionViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<DepositTransactionViewState> DepositTransactions
        {
            get { return _depositTransactions; }
            internal set { _depositTransactions = value; }
        }

        #endregion
    }
}
