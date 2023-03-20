using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class RegistrationConfirmationMethodViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private RegistrationVerificationMethod _confirmationMethod;
        #endregion

        #region PROPERTIES

        public RegistrationVerificationMethod ConfirmationMethod
        {
            get { return _confirmationMethod; }
            internal set { SetProperty(ref _confirmationMethod, value); }
        }

        #endregion
    }
}
