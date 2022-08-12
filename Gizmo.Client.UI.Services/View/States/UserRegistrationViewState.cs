using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private UserRegistrationMethod _confirmationMethod;
        #endregion

        #region PROPERTIES

        public UserRegistrationMethod ConfirmationMethod
        {
            get { return _confirmationMethod; }
            internal set { SetProperty(ref _confirmationMethod, value); }
        }

        #endregion

    }
}
