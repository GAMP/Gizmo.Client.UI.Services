﻿using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserChangePasswordViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _oldPassword = string.Empty;
        private string _newPassword = string.Empty;
        private string _repeatPassword = string.Empty;
        private bool _isComplete;
        private bool _isSuccessful;
        private string _message = string.Empty;
        #endregion

        #region PROPERTIES

        [ValidatingProperty()]
        [Required()]
        public string OldPassword
        {
            get { return _oldPassword; }
            internal set { _oldPassword = value; }
        }

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string NewPassword
        {
            get { return _newPassword; }
            internal set { _newPassword = value; }
        }

        /// <summary>
        /// Gets or sets repeat password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string RepeatPassword
        {
            get { return _repeatPassword; }
            internal set { _repeatPassword = value; }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            internal set { _isComplete = value; }
        }

        public bool IsSuccessful
        {
            get { return _isSuccessful; }
            internal set { _isSuccessful = value; }
        }

        public string Message
        {
            get { return _message; }
            internal set { _message = value; }
        }

        #endregion
    }
}
