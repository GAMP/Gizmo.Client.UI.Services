using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationConfirmationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty(IsAsync = true)]
        [Required()]
        public string ConfirmationCode { get; internal set; } = string.Empty;

        public string ConfirmationCodeMessage { get; internal set; } = string.Empty;

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
