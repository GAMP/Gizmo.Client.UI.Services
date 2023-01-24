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
        private UserLoginType _userLoginType;
        private string? _loginName;
        private string? _password;
        private bool _isPasswordVisible;
        private bool _hasLoginError;
        private string _loginError = string.Empty;
        #endregion

        #region PROPERTIES

        public bool IsLogginIn
        {
            get { return _isLogginIn; }
            internal set { SetProperty(ref _isLogginIn, value); }
        }

        /// <summary>
        /// Gets or sets login type.
        /// </summary>
        public UserLoginType LoginType 
        {
            get { return _userLoginType; }
            internal set { SetProperty(ref _userLoginType, value); }
        }

        /// <summary>
        /// Gets or sets username,email or mobile phone used for login.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? LoginName 
        {
            get { return _loginName; }
            internal set { SetProperty(ref _loginName, value); }
        }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? Password 
        {
            get { return _password; }
            internal set { SetProperty(ref _password, value); }
        }

        public bool IsPasswordVisible
        {
            get { return _isPasswordVisible; }
            internal set { SetProperty(ref _isPasswordVisible, value); }
        }

        public bool HasLoginError
        {
            get { return _hasLoginError; }
            internal set { SetProperty(ref _hasLoginError, value); }
        }

        public string LoginError
        {
            get { return _loginError; }
            internal set { SetProperty(ref _loginError, value); }
        }

        #endregion

        public override void SetDefaults()
        {
            using(PropertyChangedLock())
            {
                LoginName = null;
                Password = null;
                LoginType = UserLoginType.UsernameOrEmail;
                IsLogginIn = false;
                HasLoginError = false;
                IsPasswordVisible = false;
            }

            base.SetDefaults();
        }
    }
}
