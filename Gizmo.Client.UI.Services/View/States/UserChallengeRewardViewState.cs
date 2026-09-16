using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserChallengeRewardViewState : ViewStateBase
    {
        public ChallengeRewardKind Kind { get; internal set; }
        public string Text { get; internal set; } = string.Empty;
        public string StatusText { get; internal set; } = string.Empty;
    }
}
