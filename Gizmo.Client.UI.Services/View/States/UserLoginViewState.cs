﻿using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserLoginViewState : ValidatingViewState
    {
        private UserLoginType _userLoginType;
        private string? _loginName;
        private string? _password;

        #region PROPERTIES

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
        [Required()]
        public string? LoginName 
        {
            get { return _loginName; }
            set { SetProperty(ref _loginName, value); }
        }

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        [Required()]
        public string? Password 
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        #endregion
    }
}
