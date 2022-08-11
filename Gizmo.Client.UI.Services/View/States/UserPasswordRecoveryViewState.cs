using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public class UserPasswordRecoveryViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _mobilePhone = string.Empty;
        private string _email = string.Empty;
        #endregion

        #region PROPERTIES

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
