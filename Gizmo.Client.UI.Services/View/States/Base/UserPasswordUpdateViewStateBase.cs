using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    public abstract class UserPasswordUpdateViewStateBase : ValidatingViewState
    {
        #region FIELDS
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        [Required()]
        [DefaultValue("")]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Gets or sets confirm password.
        /// </summary>
        [Required()]
        [DefaultValue("")]
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value; }
        }

        #endregion
    }
}
