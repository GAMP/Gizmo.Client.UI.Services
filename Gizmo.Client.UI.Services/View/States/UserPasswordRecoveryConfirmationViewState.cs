using Gizmo.Client.UI;
using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    public class UserPasswordRecoveryConfirmationViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _confirmationCode = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets confirmation code.
        /// </summary>
        public string ConfirmationCode
        {
            get { return _confirmationCode; }
            set { SetProperty(ref _confirmationCode, value); }
        }

        #endregion
    }
}
