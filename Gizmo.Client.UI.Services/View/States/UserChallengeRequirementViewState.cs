using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserChallengeRequirementViewState : ViewStateBase
    {
        public int AchievementId { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public bool IsMet { get; internal set; }
    }
}
