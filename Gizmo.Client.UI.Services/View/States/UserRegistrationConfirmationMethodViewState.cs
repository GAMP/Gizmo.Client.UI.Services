using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationConfirmationMethodViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private UserRegistrationMethod _confirmationMethod;
        private string _email = String.Empty;
        private string _mobilePhone = String.Empty;
        #endregion

        #region PROPERTIES

        public UserRegistrationMethod ConfirmationMethod
        {
            get { return _confirmationMethod; }
            internal set { SetProperty(ref _confirmationMethod, value); }
        }

        [ValidatingProperty()]
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { SetProperty(ref _mobilePhone, value); }
        }

        #endregion
    }
}
