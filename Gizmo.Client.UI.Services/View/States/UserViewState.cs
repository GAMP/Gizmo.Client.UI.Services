using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _username = string.Empty;
        private DateTime _registrationDate;
        private decimal _balance;
        private string _currentTimeProduct = string.Empty;
        private TimeSpan _time;
        private int _points;
        private string _picture = string.Empty;
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

        #endregion
    }
}