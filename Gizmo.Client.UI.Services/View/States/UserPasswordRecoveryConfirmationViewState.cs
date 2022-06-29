namespace Gizmo.Client.UI.View.States
{
    public class UserPasswordRecoveryConfirmationViewState : ValidatingViewState
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
