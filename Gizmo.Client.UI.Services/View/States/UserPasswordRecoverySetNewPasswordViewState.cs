using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoverySetNewPasswordViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [ValidatingProperty()]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        [StringLength(24, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string NewPassword { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets or sets repeat password.
        /// </summary>
        [ValidatingProperty()]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        [StringLength(24, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string RepeatPassword { get; internal set; } = string.Empty;

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        public PasswordTooltipViewState PasswordTooltip { get; internal set; } = new PasswordTooltipViewState();

        #endregion
    }
}
