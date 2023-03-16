using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationConfirmationMethodViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty()]
        public string? Email { get; internal set; }

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        [ValidatingProperty()]
        public string? MobilePhone { get; internal set; }

        public bool IsLoading { get; internal set; }

        public bool CanResend { get; internal set; }

        public TimeSpan ResendTimeLeft { get; internal set; }

        #endregion
    }
}
