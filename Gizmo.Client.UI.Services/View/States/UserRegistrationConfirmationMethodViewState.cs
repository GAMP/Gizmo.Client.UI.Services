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
        private string _email = string.Empty;
        private string _mobilePhone = string.Empty;
        private bool _isLoading;
        private bool _canResend;
        private TimeSpan _resendTimeLeft;
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
            internal set { SetProperty(ref _email, value); }
        }

        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        public bool CanResend
        {
            get { return _canResend; }
            internal set { SetProperty(ref _canResend, value); }
        }

        public TimeSpan ResendTimeLeft
        {
            get { return _resendTimeLeft; }
            internal set { SetProperty(ref _resendTimeLeft, value); }
        }

        #endregion
    }
}
