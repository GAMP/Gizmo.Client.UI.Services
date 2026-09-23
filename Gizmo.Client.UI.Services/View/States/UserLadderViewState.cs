using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserLadderViewState : ViewStateBase
    {
        public bool IsLoading { get; internal set; }
        public bool HasError { get; internal set; }
        public string ErrorMessage { get; internal set; } = string.Empty;
        public bool HasStanding { get; internal set; }

        public string PeriodText { get; internal set; } = string.Empty;
        public string PeriodEndsLabelText { get; internal set; } = string.Empty;
        public string PeriodEndText { get; internal set; } = string.Empty;
        public string DaysLeftText { get; internal set; } = string.Empty;

        public int CurrentOrdinal { get; internal set; }
        public string CurrentLevelName { get; internal set; } = string.Empty;
        public string? CurrentEmblemUrl { get; internal set; }

        public bool ShowScore { get; internal set; }
        public string ScoreText { get; internal set; } = string.Empty;
        public string ScoreUnitText { get; internal set; } = string.Empty;

        public bool ShowStatusLine { get; internal set; }
        public string StatusLineText { get; internal set; } = string.Empty;

        public bool ShowKeepWarning { get; internal set; }
        public string KeepWarningText { get; internal set; } = string.Empty;

        public bool ShowProgressUpdating { get; internal set; }
        public string ProgressUpdatingText { get; internal set; } = string.Empty;

        public bool ShowProgress { get; internal set; }
        public decimal ProgressPercent { get; internal set; }
        public bool ProgressIsSecured { get; internal set; }
        public string ProgressGoalText { get; internal set; } = string.Empty;

        public bool ShowSegments { get; internal set; }
        public int SegmentCount { get; internal set; }
        public int SegmentsLit { get; internal set; }
        public string ProgressLabelText { get; internal set; } = string.Empty;
        public string ProgressCountText { get; internal set; } = string.Empty;
        public string ProgressUnitText { get; internal set; } = string.Empty;

        public bool ShowRequirements { get; internal set; }
        public string RequirementsLabelText { get; internal set; } = string.Empty;
        public IEnumerable<UserLadderRequirementViewState> Requirements { get; internal set; } = Enumerable.Empty<UserLadderRequirementViewState>();

        public bool ShowBanner { get; internal set; }
        public string BannerTitleText { get; internal set; } = string.Empty;
        public string BannerDetailText { get; internal set; } = string.Empty;

        public IEnumerable<UserLadderLevelViewState> Levels { get; internal set; } = Enumerable.Empty<UserLadderLevelViewState>();

        public int? SelectedRank { get; internal set; }
        public bool HasSelection => SelectedRank is not null;
        public string HistoryTitleText { get; internal set; } = string.Empty;
        public IEnumerable<UserLadderTransitionViewState> HistoryRows { get; internal set; } = Enumerable.Empty<UserLadderTransitionViewState>();
    }
}
