using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserLadderLevelViewState : ViewStateBase
    {
        public int Rank { get; internal set; }
        public int Ordinal { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public bool IsCurrent { get; internal set; }
        public bool IsSatisfied { get; internal set; }
        public bool IsSelected { get; internal set; }
        public string ThresholdText { get; internal set; } = string.Empty;
    }
}
