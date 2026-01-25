using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class RegistrationConfirmationMethodViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private Server.RegistrationVerificationMethod _confirmationMethod;
        #endregion

        #region PROPERTIES

        public Server.RegistrationVerificationMethod ConfirmationMethod
        {
            get { return _confirmationMethod; }
            internal set { _confirmationMethod = value; }
        }

        #endregion
    }
}
