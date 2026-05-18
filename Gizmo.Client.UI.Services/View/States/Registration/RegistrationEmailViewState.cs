using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class RegistrationEmailViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty(IsAsync = true)]
        [EmailNullEmptyValidation(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_INVALID_FIELD")]
        public string? Email { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
