using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationConfirmationMethodViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty(IsAsync = true)]
        [EmailNullEmptyValidation(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_REGISTRATION_VE_EMAIL_INVALID")]
        public string? Email { get; internal set; }

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        //public string? Prefix { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        [PhoneNullEmptyValidation(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_REGISTRATION_VE_MOBILE_PHONE_INVALID")]
        public string? MobilePhone { get; internal set; }

        public string Destination { get; internal set; } = string.Empty;

        public string? Token { get; internal set; }

        public int CodeLength { get; internal set; }

        public ConfirmationCodeDeliveryMethod DeliveryMethod { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
