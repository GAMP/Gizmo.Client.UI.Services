using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class DepositTransactionViewState : ViewStateBase
    {
        #region FIELDS
        private DateTime _transactionDate;
        private DepositTransactionType _depositTransactionType;
        private decimal _amount;
        #endregion

        #region PROPERTIES

        public DateTime TransactionDate
        {
            get { return _transactionDate; }
            internal set { _transactionDate = value; }
        }

        public DepositTransactionType DepositTransactionType
        {
            get { return _depositTransactionType; }
            internal set { _depositTransactionType = value; }
        }

        public decimal Amount
        {
            get { return _amount; }
            internal set { _amount = value; }
        }

        #endregion
    }
}
