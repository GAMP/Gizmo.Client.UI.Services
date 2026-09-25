using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserLadderTransitionViewState : ViewStateBase
    {
        public string FromName { get; internal set; } = string.Empty;
        public string ToName { get; internal set; } = string.Empty;
        public bool IsUp { get; internal set; }
        public string DateText { get; internal set; } = string.Empty;
    }
}
