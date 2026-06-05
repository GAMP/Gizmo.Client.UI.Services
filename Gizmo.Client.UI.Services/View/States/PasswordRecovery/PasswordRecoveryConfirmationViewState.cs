using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class PasswordRecoveryConfirmationViewState : ValidatingViewStateBase
    {
        [ValidatingProperty]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        public string ConfirmationCode { get; internal set; } = string.Empty;

        public string ConfirmationCodeMessage { get; internal set; } = string.Empty;

        public int SecondsLeft { get; internal set; }

        public bool TimerExpired => SecondsLeft <= 0;

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;
    }
}
