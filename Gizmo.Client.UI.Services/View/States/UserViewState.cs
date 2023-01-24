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
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime? _birthDate;
        private Sex _sex = Sex.Unspecified;
        private string _country = string.Empty;
        private string _address = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _mobilePhone = string.Empty;
        private DateTime _registrationDate;
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

        public string FirstName
        {
            get { return _firstName; }
            internal set { SetProperty(ref _firstName, value); }
        }

        public string LastName
        {
            get { return _lastName; }
            internal set { SetProperty(ref _lastName, value); }
        }

        public DateTime? BirthDate
        {
            get { return _birthDate; }
            internal set { SetProperty(ref _birthDate, value); }
        }

        public Sex Sex
        {
            get { return _sex; }
            internal set { SetProperty(ref _sex, value); }
        }

        public string Country
        {
            get { return _country; }
            internal set { SetProperty(ref _country, value); }
        }

        public string Address
        {
            get { return _address; }
            internal set { SetProperty(ref _address, value); }
        }

        public string Email
        {
            get { return _email; }
            internal set { SetProperty(ref _email, value); }
        }

        public string Phone
        {
            get { return _phone; }
            internal set { SetProperty(ref _phone, value); }
        }

        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        public DateTime RegistrationDate
        {
            get { return _registrationDate; }
            internal set { SetProperty(ref _registrationDate, value); }
        }

        public string Picture
        {
            get { return _picture; }
            internal set { SetProperty(ref _picture, value); }
        }

        #endregion
    }
}