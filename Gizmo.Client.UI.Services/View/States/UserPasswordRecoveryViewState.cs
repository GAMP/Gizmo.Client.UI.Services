using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public UserRecoveryMethod SelectedRecoveryMethod { get; internal set; }

        [ValidatingProperty()]
        [Phone()]
        public string MobilePhone { get; internal set; } = string.Empty;

        [ValidatingProperty()]
        [EmailAddress()]
        public string Email { get; internal set; } = string.Empty;

        public string Destination { get; internal set; } = string.Empty;

        public string Token { get; internal set; } = string.Empty;

        public int CodeLength { get; internal set; }

        public ConfirmationCodeDeliveryMethod DeliveryMethod { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
