using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryMethodServiceViewState : ViewStateBase
    {
        public UserRecoveryMethod AvailabledRecoveryMethod { get; internal set; }
    }
}
