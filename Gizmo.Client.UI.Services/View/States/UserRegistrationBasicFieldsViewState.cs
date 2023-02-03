using Gizmo.UI;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationBasicFieldsViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _username = string.Empty;
        private string _newPassword = string.Empty;
        private string _repeatPassword = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string Username
        {
            get { return _username; }
            internal set { SetProperty(ref _username, value); }
        }

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string NewPassword
        {
            get { return _newPassword; }
            internal set { SetProperty(ref _newPassword, value); }
        }

        /// <summary>
        /// Gets or sets repeat password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string RepeatPassword
        {
            get { return _repeatPassword; }
            internal set { SetProperty(ref _repeatPassword, value); }
        }

        public UserModelRequiredInfo DefaultUserGroupRequiredInfo { get; internal set; }

        #endregion
    }
}
