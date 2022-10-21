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
        #endregion

        #region PROPERTIES

        public UserPasswordRecoveryMethod Method
        {
            get { return _method; }
            set { SetProperty(ref _method, value); }
        }

        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { SetProperty(ref _mobilePhone, value); }
        }

        [ValidatingProperty()]
        [Required()]
        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        }

        #endregion
    }
}
