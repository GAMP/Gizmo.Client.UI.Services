using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _username = String.Empty;
        private DateTime _registrationDate;
        private decimal _balance;
        private string _currentTimeProduct = String.Empty;
        private TimeSpan _time;
        private int _points;
        private string _picture = String.Empty;
        private bool _isLocked;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { SetProperty(ref _id, value); }
        }

        public string Username
        {
            get { return _username; }
            internal set { SetProperty(ref _username, value); }
        }

        public DateTime RegistrationDate
        {
            get { return _registrationDate; }
            internal set { SetProperty(ref _registrationDate, value); }
        }

        public decimal Balance
        {
            get { return _balance; }
            internal set { SetProperty(ref _balance, value); }
        }

        public string CurrentTimeProduct
        {
            get { return _currentTimeProduct; }
            internal set { SetProperty(ref _currentTimeProduct, value); }
        }

        public TimeSpan Time
        {
            get { return _time; }
            internal set { SetProperty(ref _time, value); }
        }

        public int Points
        {
            get { return _points; }
            internal set { SetProperty(ref _points, value); }
        }

        public string Picture
        {
            get { return _picture; }
            internal set { SetProperty(ref _picture, value); }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { SetProperty(ref _isLocked, value); }
        }

        #endregion
    }
}