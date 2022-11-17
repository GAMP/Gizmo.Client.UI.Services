using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserBalanceViewState : ViewStateBase
    {
        #region FIELDS
        private decimal _balance;
        private int _pointsBalance;
        private decimal _outstanding;
        #endregion

        #region PROPERTIES
        
        [DefaultValue(0)]
        public decimal Balance
        {
            get { return _balance; }
            internal set { SetProperty(ref _balance, value); }
        }
        
        [DefaultValue(0)]
        public int PointsBalance
        {
            get { return _pointsBalance; }
            internal set { SetProperty(ref _pointsBalance, value); }
        }

        [DefaultValue(0)]
        public decimal Outstanding
        {
            get { return _outstanding; }
            internal set { SetProperty(ref _outstanding, value); }
        }

        #endregion
    }
}
