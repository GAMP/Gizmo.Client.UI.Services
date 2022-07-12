using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public class UserPasswordRecoveryViewState : ViewStateBase
    {
        #region FIELDS
        private string _mobilePhone = string.Empty;
        private string _email = string.Empty;
        #endregion

        #region PROPERTIES
        
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set
            {
                SetProperty(ref _mobilePhone, value);
            }
        }

        public string Email
        {
            get { return _email; }
            set { SetProperty(ref _email, value); }
        } 

        #endregion
    }
}
