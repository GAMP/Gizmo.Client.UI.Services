using Gizmo.Client.UI.Services;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserChallengeViewState : ViewStateBase
    {
        public int ChallengeId { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public ChallengeState State { get; internal set; }

        public bool IsDone { get; internal set; }
        public bool IsEnded { get; internal set; }
        public bool HasChip { get; internal set; }
        public bool ChipIsSuccess { get; internal set; }
        public string ChipText { get; internal set; } = string.Empty;
        public bool ShowProgressBar { get; internal set; }
        public decimal ProgressPercent { get; internal set; }
        public string CountText { get; internal set; } = string.Empty;
        public string WindowText { get; internal set; } = string.Empty;
        public bool WindowIsWarning { get; internal set; }
        public string RequirementsText { get; internal set; } = string.Empty;

        public string PopupDescription { get; internal set; } = string.Empty;
        public string PopupRequirementsCountText { get; internal set; } = string.Empty;
        public IReadOnlyList<UserChallengeRequirementViewState> Requirements { get; internal set; } = Array.Empty<UserChallengeRequirementViewState>();
        public IReadOnlyList<UserChallengeRewardViewState> Rewards { get; internal set; } = Array.Empty<UserChallengeRewardViewState>();
        public string PopupWindowText { get; internal set; } = string.Empty;
        public bool PopupWindowIsWarning { get; internal set; }
    }
}
