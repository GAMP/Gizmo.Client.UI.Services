using System.ComponentModel.DataAnnotations;
using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationPhoneViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        [ValidatingProperty()]
        public string? RegionCode { get; internal set; }

        [ValidatingProperty(IsAsync = true)]
        public string? MobilePhone { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool HasError { get; internal set; }

        public string ErrorMessage { get; internal set; } = string.Empty;

        #endregion
    }
}
