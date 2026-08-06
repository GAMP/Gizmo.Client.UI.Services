using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class PasswordRecoveryKindViewState : ViewStateBase
    {
        public bool IsLoading { get; internal set; }

        public bool CanUseEmail { get; internal set; }

        public bool CanUseMobilePhone { get; internal set; }
    }
}
