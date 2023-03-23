using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserVerificationFallbackViewState : ViewStateBase
    {
        #region PROPERTIES

        public bool IsSMSFallbackAvailable { get; internal set; }

        public bool IsVerificationFallbackLocked { get; internal set; }

        public TimeSpan Countdown { get; internal set; }

        #endregion
    }
}
