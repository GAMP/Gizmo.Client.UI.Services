using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationAdditionalFieldsViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        public string? Prefix { get; internal set; }

        /// <summary>
        /// Gets or sets mobile phone.
        /// </summary>
        [ValidatingProperty()]
        [PhoneNullEmptyValidation()]
        public string? MobilePhone { get; internal set; }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        [ValidatingProperty()]
        public string? Address { get; internal set; }

        /// <summary>
        /// Gets or sets post code.
        /// </summary>
        [ValidatingProperty()]
        public string? PostCode { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
