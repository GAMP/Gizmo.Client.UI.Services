using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserLoginViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private bool _isLogginIn;
        private bool _isLogginOut;
        private UserLoginType _userLoginType;
#if RELEASE
        private string? _loginName;
        private string? _password; 
#else
        private string? _loginName = "user";
        private string? _password = "user";
#endif
        private bool _isPasswordVisible;
        private bool _hasLoginError;
        private string? _loginError;
        #endregion

        #region PROPERTIES

        public bool IsLogginIn
        {
            get { return _isLogginIn; }
            internal set { _isLogginIn = value; }
        }

        public bool IsLogginOut
        {
            get { return _isLogginOut; }
            internal set { _isLogginOut = value; }
        }

        /// <summary>
        /// Gets or sets login type.
        /// </summary>
        public UserLoginType LoginType
        {
            get { return _userLoginType; }
            internal set { _userLoginType = value; }
        }

        /// <summary>
        /// Gets or sets username,email or mobile phone used for login.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? LoginName
        {
            get { return _loginName; }
            internal set { _loginName = value; }
        }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        [ValidatingProperty()]
        public string? Password
        {
            get { return _password; }
            internal set { _password = value; }
        }

        public bool IsPasswordVisible
        {
            get { return _isPasswordVisible; }
            internal set { _isPasswordVisible = value; }
        }

        public bool HasLoginError
        {
            get { return _hasLoginError; }
            internal set { _hasLoginError = value; }
        }

        public string? LoginError
        {
            get { return _loginError; }
            internal set { _loginError = value; }
        }

        #endregion

        public override void SetDefaults()
        {
            LoginName = null;
            Password = null;
            LoginType = UserLoginType.UsernameOrEmail;
            IsLogginIn = false;
            HasLoginError = false;
            IsPasswordVisible = false;
            LoginError = null;
            base.SetDefaults();
        }
    }
}
