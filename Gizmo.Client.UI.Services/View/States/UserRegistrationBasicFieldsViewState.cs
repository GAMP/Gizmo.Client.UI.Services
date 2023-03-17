using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationBasicFieldsViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string? _username;
        private string? _password;
        private string? _repeatPassword;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? Username
        {
            get { return _username; }
            internal set { SetProperty(ref _username, value); }
        }

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? Password
        {
            get { return _password; }
            internal set { SetProperty(ref _password, value); }
        }

        /// <summary>
        /// Gets or sets repeat password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? RepeatPassword
        {
            get { return _repeatPassword; }
            internal set { SetProperty(ref _repeatPassword, value); }
        }

        [ValidatingProperty()]
        public string? FirstName { get; internal set; }

        [ValidatingProperty()]
        public string? LastName { get; internal set; }

        [ValidatingProperty()]
        public DateTime? BirthDate { get; internal set; }

        [ValidatingProperty()]
        public Sex Sex { get; internal set; }

        [ValidatingProperty()]
        public string? Email { get; internal set; }

        #endregion
    }
}
