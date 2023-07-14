using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryConfirmationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets confirmation code.
        /// </summary>
        [ValidatingProperty(IsAsync = true)]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        public string ConfirmationCode { get; internal set; } = string.Empty;

        public string ConfirmationCodeMessage { get; internal set; } = string.Empty;

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
