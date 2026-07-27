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
        [StringLength(45, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? Country { get; internal set; }

        //public string? Prefix { get; internal set; }

        /// <summary>
        /// Gets or sets mobile phone.
        /// </summary>
        [PhoneNullEmptyValidation(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_INVALID_FIELD")]
        public string? MobilePhone { get; internal set; }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        [ValidatingProperty()]
        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? Address { get; internal set; }

        /// <summary>
        /// Gets or sets city.
        /// </summary>
        [ValidatingProperty()]
        [StringLength(45, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? City { get; internal set; }

        /// <summary>
        /// Gets or sets post code.
        /// </summary>
        [ValidatingProperty()]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? PostCode { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
