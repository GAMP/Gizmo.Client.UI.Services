using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserChangeProfileViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty()]
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        public string? Username { get; internal set; }

        [ValidatingProperty()]
        public string? FirstName { get; internal set; }

        [ValidatingProperty()]
        public string? LastName { get; internal set; }

        [ValidatingProperty()]
        public DateTime? BirthDate { get; internal set; }

        [ValidatingProperty()]
        public Sex Sex { get; internal set; }

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        public string? Picture { get; internal set; }

        public UserInfoTypes? RequiredUserInformation { get; internal set; }

        public bool IsComplete { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
