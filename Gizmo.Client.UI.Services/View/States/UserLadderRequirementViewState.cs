using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserLadderRequirementViewState : ViewStateBase
    {
        public int AchievementId { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public bool IsMet { get; internal set; }
        public bool IsLink { get; internal set; }
        public string CountText { get; internal set; } = string.Empty;
    }
}
