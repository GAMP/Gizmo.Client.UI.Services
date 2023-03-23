using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserVerificationViewState : ViewStateBase
    {
        #region PROPERTIES

        public bool IsVerificationLocked { get; internal set; }

        public TimeSpan Countdown { get; internal set; }

        #endregion
    }
}
