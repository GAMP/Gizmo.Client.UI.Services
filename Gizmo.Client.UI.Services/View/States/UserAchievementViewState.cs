using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserAchievementViewState : ViewStateBase
    {
        public int AchievementId { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public string? ImageUrl { get; internal set; }
        public bool ImageIsSvg { get; internal set; }
        public AchievementState State { get; internal set; }
        public int TotalCompletions { get; internal set; }

        public bool IsEarned { get; internal set; }
        public bool HasChip { get; internal set; }
        public bool ShowProgressBar { get; internal set; }
        public decimal ProgressPercent { get; internal set; }
        public string CountText { get; internal set; } = string.Empty;
        public string StandingText { get; internal set; } = string.Empty;
        public string ChipText { get; internal set; } = string.Empty;

        public bool PopupIsCompleted { get; internal set; }
        public string PopupDescription { get; internal set; } = string.Empty;
        public string PopupProgressText { get; internal set; } = string.Empty;
        public string PopupResetText { get; internal set; } = string.Empty;
        public string PopupCompletedText { get; internal set; } = string.Empty;
    }
}
