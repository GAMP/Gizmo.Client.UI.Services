using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserChallengeRewardViewState : ViewStateBase
    {
        public ChallengeRewardKind Kind { get; internal set; }
        /// <summary>"50 Points" / "60 min Time".</summary>
        public string Text { get; internal set; } = string.Empty;
        /// <summary>Localized grant status; empty when the challenge is not done.</summary>
        public string StatusText { get; internal set; } = string.Empty;
    }
}
