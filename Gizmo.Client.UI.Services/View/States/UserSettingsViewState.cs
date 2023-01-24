using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserSettingsViewState : ValidatingViewStateBase
    {
        #region FIELDS
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
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string Username
        {
            get { return _username; }
            internal set { SetProperty(ref _username, value); }
        }

        [ValidatingProperty()]
        public string FirstName
        {
            get { return _firstName; }
            internal set { SetProperty(ref _firstName, value); }
        }

        [ValidatingProperty()]
        public string LastName
        {
            get { return _lastName; }
            internal set { SetProperty(ref _lastName, value); }
        }

        [ValidatingProperty()]
        public DateTime? BirthDate
        {
            get { return _birthDate; }
            internal set { SetProperty(ref _birthDate, value); }
        }

        [ValidatingProperty()]
        public Sex Sex
        {
            get { return _sex; }
            internal set { SetProperty(ref _sex, value); }
        }

        [ValidatingProperty()]
        public string Country
        {
            get { return _country; }
            internal set { SetProperty(ref _country, value); }
        }

        [ValidatingProperty()]
        public string Address
        {
            get { return _address; }
            internal set { SetProperty(ref _address, value); }
        }

        [ValidatingProperty()]
        public string Email
        {
            get { return _email; }
            internal set { SetProperty(ref _email, value); }
        }

        [ValidatingProperty()]
        public string Phone
        {
            get { return _phone; }
            internal set { SetProperty(ref _phone, value); }
        }

        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        #endregion
    }
}