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
        private TimeSpan _time;
        #endregion

        #region PROPERTIES

        [DefaultValue(0)]
        public decimal Balance
        {
            get { return _balance; }
            internal set { _balance = value; }
        }

        [DefaultValue(0)]
        public int PointsBalance
        {
            get { return _pointsBalance; }
            internal set { _pointsBalance = value; }
        }

        [DefaultValue(0)]
        public decimal Outstanding
        {
            get { return _outstanding; }
            internal set { _outstanding = value; }
        }

        public TimeSpan Time
        {
            get { return _time; }
            internal set { _time = value; }
        }

        #endregion
    }
}
