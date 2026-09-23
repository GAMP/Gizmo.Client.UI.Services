using System.Globalization;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserLadderRoute)]
    public sealed class UserLadderViewService : ViewStateServiceBase<UserLadderViewState>
    {
        public UserLadderViewService(UserLadderViewState viewState,
            IUserLadderService ladderService,
            IUserLadderStandingContext standingContext,
            ILocalizationService localizationService,
            ILogger<UserLadderViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _ladderService = ladderService;
            _standingContext = standingContext;
            _localizationService = localizationService;
        }

        private readonly IUserLadderService _ladderService;
        private readonly IUserLadderStandingContext _standingContext;
        private readonly ILocalizationService _localizationService;
        private UserLadderStanding? _standing;
        private IReadOnlyList<UserLadderTransition> _transitions = Array.Empty<UserLadderTransition>();
        private int? _selectedRank;

        public async Task LoadAsync(CancellationToken cToken = default)
        {
            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var standingTask = _standingContext.RefreshAsync(cToken);
                var transitionsTask = LoadTransitionsAsync(cToken);

                await Task.WhenAll(standingTask, transitionsTask);

                _standing = _standingContext.Standing;
                _transitions = await transitionsTask ?? _standing?.Transitions ?? Array.Empty<UserLadderTransition>();

                Apply();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load user ladder standing.");
                _standing = null;
                _transitions = Array.Empty<UserLadderTransition>();
                Apply();
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        private async Task<IReadOnlyList<UserLadderTransition>?> LoadTransitionsAsync(CancellationToken cToken)
        {
            try
            {
                return await _ladderService.GetTransitionsAsync(cToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to load user ladder transitions, falling back to the standing's newest transitions.");
                return null;
            }
        }

        public void SelectLevel(int rank)
        {
            if (_selectedRank == rank)
                return;

            _selectedRank = rank;
            ApplySelection();
            ViewState.RaiseChanged();
        }

        public void ClearSelection()
        {
            if (_selectedRank is null)
                return;

            _selectedRank = null;
            ApplySelection();
            ViewState.RaiseChanged();
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _selectedRank = null;
            return LoadAsync(cToken);
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            base.OnDisposing(isDisposing);
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            Apply();

            if (ViewState.HasError)
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));

            ViewState.RaiseChanged();
        }

        private void Apply()
        {
            ApplyStanding();
            ApplyLevels();
            ApplyRequirements();
            ApplyHistory();
        }

        private void ApplySelection()
        {
            foreach (var level in ViewState.Levels)
                level.IsSelected = level.Rank == _selectedRank;

            ApplyHistory();
        }

        private void ApplyStanding()
        {
            var s = _standing;
            ViewState.HasStanding = s is not null;

            if (s is null)
            {
                ViewState.PeriodText = ViewState.PeriodEndsLabelText = ViewState.PeriodEndText = ViewState.DaysLeftText = string.Empty;
                ViewState.CurrentOrdinal = 0;
                ViewState.CurrentLevelName = string.Empty;
                ViewState.CurrentEmblemUrl = null;
                ViewState.ShowScore = ViewState.ShowProgress = ViewState.ShowBanner = false;
                ViewState.ScoreText = ViewState.ScoreUnitText = ViewState.ProgressGoalText = ViewState.BannerTitleText = ViewState.BannerDetailText = string.Empty;
                ViewState.ProgressPercent = 0m;
                ViewState.ProgressIsSecured = false;
                ViewState.ShowStatusLine = ViewState.ShowSegments = ViewState.ShowRequirements = ViewState.ShowProgressUpdating = ViewState.ShowKeepWarning = false;
                ViewState.StatusLineText = ViewState.ProgressLabelText = ViewState.ProgressCountText = ViewState.ProgressUnitText = ViewState.RequirementsLabelText = ViewState.ProgressUpdatingText = ViewState.KeepWarningText = string.Empty;
                ViewState.SegmentCount = ViewState.SegmentsLit = 0;
                ViewState.Requirements = Enumerable.Empty<UserLadderRequirementViewState>();
                return;
            }

            var current = s.CurrentLevel();
            var next = s.NextLevel();
            bool secured = s.IsSecured();
            bool atEntry = s.IsAtEntry();

            bool isRequirements = s.Mode == LadderMode.Requirements;
            var target = next;
            int? total = target?.Requirements?.Count;
            int? met = target?.MetCount;
            bool hasTarget = isRequirements && current is not null && target is not null && total is > 0;

            ViewState.PeriodText = GetPeriodText(s.PeriodKind);
            ViewState.PeriodEndsLabelText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_ENDS));
            ViewState.PeriodEndText = ProfileDateFormat.MonthDay(s.PeriodEnd);
            int daysLeft = Math.Max(0, (int)Math.Ceiling((s.PeriodEnd - DateTime.UtcNow).TotalDays));
            ViewState.DaysLeftText = daysLeft == 0
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_ENDS_TODAY))
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_DAYS_LEFT), daysLeft);

            ViewState.CurrentOrdinal = s.OrdinalOf(s.CurrentRank);
            ViewState.CurrentLevelName = current?.Name ?? s.LevelNameByRank(s.CurrentRank);
            ViewState.CurrentEmblemUrl = current?.EmblemUrl;

            ViewState.ShowScore = s.Mode == LadderMode.Points && s.Score is not null;
            ViewState.ScoreText = s.Score is decimal score ? AchievementValueFormat.Trim(decimal.Floor(score)) : string.Empty;
            ViewState.ScoreUnitText = GetScoreUnitText(s.PeriodKind);

            ViewState.StatusLineText = UserLadderStatusText.RequirementsStatus(_localizationService, s);
            ViewState.ShowStatusLine = ViewState.StatusLineText.Length > 0;

            ViewState.KeepWarningText = UserLadderStatusText.KeepWarning(_localizationService, s);
            ViewState.ShowKeepWarning = ViewState.KeepWarningText.Length > 0;

            ViewState.ShowProgressUpdating = !s.IsFrozen && s.State is null && current is not null;
            ViewState.ProgressUpdatingText = ViewState.ShowProgressUpdating
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROGRESS_UPDATING))
                : string.Empty;

            int? keep = (atEntry || s.IsOnlyLevel()) ? null : current?.Threshold;
            if (keep is <= 0)
                keep = null;
            int? reach = next?.Threshold;
            if (reach is <= 0)
                reach = null;

            int? scale;
            bool goalIsReach;
            if (secured || atEntry)
            {
                goalIsReach = reach is not null;
                scale = reach ?? keep;
            }
            else
            {
                goalIsReach = keep is null && reach is not null;
                scale = keep ?? reach;
            }

            ViewState.ShowProgress = ViewState.ShowScore && scale is not null;
            ViewState.ProgressPercent = ViewState.ShowProgress
                ? Math.Clamp(s.Score!.Value / scale!.Value * 100m, 0m, 100m)
                : 0m;
            ViewState.ProgressIsSecured = secured;
            ViewState.ProgressGoalText = scale is null
                ? string.Empty
                : goalIsReach
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_GOAL_REACH), AchievementValueFormat.Trim(scale.Value), next!.Name)
                    : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_GOAL_RETAIN), AchievementValueFormat.Trim(scale.Value));

            ViewState.ShowSegments = hasTarget && met is not null;
            ViewState.SegmentCount = ViewState.ShowSegments ? total!.Value : 0;
            ViewState.SegmentsLit = ViewState.ShowSegments ? Math.Clamp(met!.Value, 0, ViewState.SegmentCount) : 0;
            ViewState.ProgressLabelText = ViewState.ShowSegments
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROGRESS_TO), target!.Name)
                : string.Empty;
            ViewState.ProgressCountText = ViewState.ShowSegments
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_MET_COUNT), ViewState.SegmentsLit, ViewState.SegmentCount)
                : string.Empty;
            ViewState.ProgressUnitText = ViewState.ShowSegments
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_ACHIEVEMENTS_UNIT))
                : string.Empty;

            ViewState.ShowBanner = !isRequirements && secured && current is not null;
            ViewState.BannerTitleText = current is null
                ? string.Empty
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_SECURED), current.Name);
            ViewState.BannerDetailText = next is null
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_TOP))
                : s.Score is decimal sc && next.Threshold is int t
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_NEXT), AchievementValueFormat.Trim(decimal.Ceiling(Math.Max(0m, t - sc))), next.Name)
                    : string.Empty;
        }

        private void ApplyLevels()
        {
            var s = _standing;
            if (s is null)
            {
                ViewState.Levels = Enumerable.Empty<UserLadderLevelViewState>();
                return;
            }

            bool isRequirements = s.Mode == LadderMode.Requirements;
            var next = s.NextLevel();
            int nextIndex = -1;
            if (next is not null)
            {
                for (int index = 0; index < s.Levels.Count; index++)
                {
                    if (s.Levels[index].Rank == next.Rank)
                    {
                        nextIndex = index;
                        break;
                    }
                }
            }

            ViewState.Levels = s.Levels.Select((level, index) =>
            {
                var perkTexts = level.Perks.Select(GetPerkText).Where(text => text.Length > 0).ToList();
                bool hasDescription = !string.IsNullOrWhiteSpace(level.Description);

                return new UserLadderLevelViewState
                {
                    Rank = level.Rank,
                    Ordinal = index + 1,
                    Name = level.Name,
                    IsCurrent = level.Rank == s.CurrentRank,
                    IsSatisfied = level.IsSatisfied == true,
                    IsSelected = level.Rank == _selectedRank,
                    MetaText = isRequirements
                        ? (level.Requirements is { Count: > 0 } requirements && level.MetCount is int metCount
                            ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_MET_COUNT), metCount, requirements.Count)
                            : string.Empty)
                        : (level.Threshold is int threshold && threshold > 0
                            ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_LEVEL_POINTS), AchievementValueFormat.Trim(threshold))
                            : string.Empty),
                    IsNext = isRequirements && nextIndex >= 0 && index == nextIndex,
                    IsLocked = isRequirements && nextIndex >= 0 && index > nextIndex,
                    EmblemUrl = level.EmblemUrl,
                    Description = level.Description?.Trim() ?? string.Empty,
                    HasDescription = hasDescription,
                    PerkTexts = perkTexts,
                    HasPerks = perkTexts.Count > 0,
                    HasInfo = hasDescription || perkTexts.Count > 0,
                };
            }).ToList();
        }

        // discount: operator-authored name and magnitude, "+" marks a bonus (manager convention)
        private string GetPerkText(UserLadderPerk perk)
        {
            if (perk.Kind == LadderPerkKind.WaitingLine)
                return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERK_QUEUE_PRIORITY), perk.Priority);

            string magnitude = (perk.IsBonus ? "+" : string.Empty)
                + (perk.IsPercentage ? AchievementValueFormat.Trim(perk.Value) + "%" : perk.Value.ToString("C2", CultureInfo.CurrentCulture));

            return string.IsNullOrWhiteSpace(perk.Name)
                ? magnitude
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERK_DISCOUNT), perk.Name.Trim(), magnitude);
        }

        private void ApplyRequirements()
        {
            var s = _standing;
            var target = s?.NextLevel();
            var requirements = s is not null && s.Mode == LadderMode.Requirements && s.CurrentLevel() is not null && UserLadderStatusText.IsProgressCollected(s)
                ? target?.Requirements
                : null;

            if (s is null || target is null || requirements is not { Count: > 0 })
            {
                ViewState.ShowRequirements = false;
                ViewState.RequirementsLabelText = string.Empty;
                ViewState.Requirements = Enumerable.Empty<UserLadderRequirementViewState>();
                return;
            }

            var rows = requirements
                .Select(requirement => new { requirement, achievement = s.FindAchievement(requirement.AchievementId) })
                .Where(row => row.achievement is not null)
                .Select(row =>
                {
                    bool isMet = row.achievement!.CompletedCount >= row.requirement.RequiredCount;

                    return new UserLadderRequirementViewState
                    {
                        AchievementId = row.requirement.AchievementId,
                        Name = row.achievement.Name,
                        IsMet = isMet,
                        IsLink = !isMet && !row.achievement.IsHidden,
                        CountText = row.requirement.RequiredCount > 1 && row.achievement.CompletedCount is int done
                            ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_MET_COUNT), Math.Min(done, row.requirement.RequiredCount), row.requirement.RequiredCount)
                            : string.Empty,
                        // CurrentValue is the raw value of the whole range instance, not the progress of the next
                        // completion: at or above the target (a completion already earned) it says nothing new
                        ValueText = !isMet && !row.achievement.IsHidden && row.achievement.CurrentValue is decimal current
                                && row.achievement.TargetValue > 0 && current < row.achievement.TargetValue
                            ? AchievementValueFormat.Pair(Math.Max(current, 0m), row.achievement.TargetValue, row.achievement.Unit, _localizationService)
                            : string.Empty,
                    };
                })
                .ToList();

            if (rows.Count == 0)
            {
                ViewState.ShowRequirements = false;
                ViewState.RequirementsLabelText = string.Empty;
                ViewState.Requirements = Enumerable.Empty<UserLadderRequirementViewState>();
                return;
            }

            ViewState.ShowRequirements = true;
            ViewState.RequirementsLabelText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_REQUIREMENTS_FOR), target.Name);
            ViewState.Requirements = rows;
        }

        private void ApplyHistory()
        {
            var s = _standing;
            if (s is null || _selectedRank is not int rank)
            {
                ViewState.SelectedRank = null;
                ViewState.HistoryTitleText = string.Empty;
                ViewState.HistoryRows = Enumerable.Empty<UserLadderTransitionViewState>();
                return;
            }

            var events = _transitions;
            int landedIndex = -1;
            for (int index = 0; index < events.Count; index++)
            {
                if (events[index].ToRank == rank)
                {
                    landedIndex = index;
                    break;
                }
            }

            int searchEnd = landedIndex < 0 ? events.Count : landedIndex;
            UserLadderTransition? left = null;
            for (int index = searchEnd - 1; index >= 0; index--)
            {
                if (events[index].FromRank == rank)
                {
                    left = events[index];
                    break;
                }
            }

            var rows = new List<UserLadderTransition>();
            if (left is not null)
                rows.Add(left);
            if (landedIndex >= 0)
                rows.AddRange(events.Skip(landedIndex));

            string levelName = s.LevelNameByRank(rank);

            ViewState.SelectedRank = rank;
            ViewState.HistoryTitleText = landedIndex >= 0
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_HISTORY_SINCE), levelName, ProfileDateFormat.MonthDayYear(events[landedIndex].Time))
                : levelName;
            ViewState.HistoryRows = rows.Select(e => new UserLadderTransitionViewState
            {
                FromName = s.LevelNameByRank(e.FromRank),
                ToName = s.LevelNameByRank(e.ToRank),
                IsUp = e.ToRank > e.FromRank,
                DateText = ProfileDateFormat.MonthDayYear(e.Time),
            }).ToList();
        }

        private string GetPeriodText(LadderPeriod period) => period switch
        {
            LadderPeriod.Day     => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_DAY)),
            LadderPeriod.Week    => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_WEEK)),
            LadderPeriod.Month   => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_MONTH)),
            LadderPeriod.Quarter => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_QUARTER)),
            LadderPeriod.Year    => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_YEAR)),
            _                    => string.Empty,
        };

        private string GetScoreUnitText(LadderPeriod period) => period switch
        {
            LadderPeriod.Day     => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_DAY)),
            LadderPeriod.Week    => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_WEEK)),
            LadderPeriod.Month   => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_MONTH)),
            LadderPeriod.Quarter => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_QUARTER)),
            LadderPeriod.Year    => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_SCORE_UNIT_YEAR)),
            _                    => string.Empty,
        };
    }
}
