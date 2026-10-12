using Gizmo.UI;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserChangeProfileViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty()]
        [StringLength(45, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? FirstName { get; internal set; }

        [ValidatingProperty()]
        [StringLength(45, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? LastName { get; internal set; }

        [ValidatingProperty()]
        public DateTime? BirthDate { get; internal set; }

        [ValidatingProperty()]
        public Sex Sex { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        [EmailNullEmptyValidation(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_INVALID_FIELD")]
        [StringLength(254, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? Email { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        public string? MobilePhone { get; internal set; }

        [ValidatingProperty()]
        public string? PhoneRegionCode { get; internal set; }

        [ValidatingProperty()]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? Phone { get; internal set; }

        [ValidatingProperty()]
        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? Address { get; internal set; }

        [ValidatingProperty()]
        [StringLength(45, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? City { get; internal set; }

        [ValidatingProperty()]
        [StringLength(20, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        [RegularExpression("^[0-9]*$", ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_INVALID_FIELD")]
        public string? PostCode { get; internal set; }

        [ValidatingProperty()]
        [StringLength(45, ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_MAX_LENGTH")]
        public string? Country { get; internal set; }

        public bool ShowFirstName { get; internal set; }

        public bool ShowLastName { get; internal set; }

        public bool ShowBirthDate { get; internal set; }

        public bool ShowSex { get; internal set; }

        public bool ShowEmail { get; internal set; }

        public bool ShowMobilePhone { get; internal set; }

        public bool ShowPhone { get; internal set; }

        public bool ShowAddress { get; internal set; }

        public bool ShowCity { get; internal set; }

        public bool ShowPostCode { get; internal set; }

        public bool ShowCountry { get; internal set; }

        public bool IsComplete { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
