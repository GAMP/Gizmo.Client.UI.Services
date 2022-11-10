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
        private bool _hasLoginErrors;
        #endregion

        #region PROPERTIES

        public bool IsLogginIn
        {
            get { return _isLogginIn; }
            set { SetProperty(ref _isLogginIn, value); }
        }

        /// <summary>
        /// Gets or sets login type.
        /// </summary>
        public UserLoginType LoginType 
        {
            get { return _userLoginType; }
            set { SetProperty(ref _userLoginType, value); }
        }

        /// <summary>
        /// Gets or sets username,email or mobile phone used for login.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? LoginName 
        {
            get { return _loginName; }
            set { SetProperty(ref _loginName, value); }
        }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? Password 
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public bool HasLoginErrors
        {
            get { return _hasLoginErrors; }
            set { SetProperty(ref _hasLoginErrors, value); }
        }

        #endregion

        public override void SetDefaults()
        {
            using(PropertyChangedLock())
            {
                LoginName = null;
                Password = null;
                LoginType = UserLoginType.UsernameOrEmail;
            }

            base.SetDefaults();
        }
    }
}
