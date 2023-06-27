using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationBasicFieldsViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty(IsAsync = true)]
        [Required()]
        public string Username { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        [StringLength(24)]
        public string Password { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets or sets repeat password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        [StringLength(24)]
        public string RepeatPassword { get; internal set; } = string.Empty;

        [ValidatingProperty()]
        public string? FirstName { get; internal set; }

        [ValidatingProperty()]
        public string? LastName { get; internal set; }

        [ValidatingProperty()]
        public DateTime? BirthDate { get; internal set; }

        [ValidatingProperty()]
        public Sex Sex { get; internal set; }

        [ValidatingProperty()]
        [EmailNullEmptyValidation()]
        public string? Email { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public PasswordTooltipViewState PasswordTooltip { get; internal set; } = new PasswordTooltipViewState();

        #endregion
    }
}
