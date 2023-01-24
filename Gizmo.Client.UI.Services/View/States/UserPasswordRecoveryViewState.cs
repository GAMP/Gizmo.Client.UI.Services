using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private UserPasswordRecoveryMethod _method;
        private string _mobilePhone = string.Empty;
        private string _email = string.Empty;
        private bool _isLoading;
        #endregion

        #region PROPERTIES

        public UserPasswordRecoveryMethod Method
        {
            get { return _method; }
            internal set { SetProperty(ref _method, value); }
        }

        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        [ValidatingProperty()]
        public string Email
        {
            get { return _email; }
            internal set { SetProperty(ref _email, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        #endregion
    }
}
