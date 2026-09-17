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
            ILocalizationService localizationService,
            ILogger<UserLadderViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _ladderService = ladderService;
            _localizationService = localizationService;
        }

        private readonly IUserLadderService _ladderService;
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
                var standingTask = _ladderService.GetStandingAsync(cToken);
                var transitionsTask = LoadTransitionsAsync(cToken);

                await Task.WhenAll(standingTask, transitionsTask);

                _standing = await standingTask;
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
                ViewState.ShowScore = ViewState.ShowProgress = ViewState.ShowBanner = false;
                ViewState.ScoreText = ViewState.ScoreUnitText = ViewState.ProgressGoalText = ViewState.BannerTitleText = ViewState.BannerDetailText = string.Empty;
                ViewState.ProgressPercent = 0m;
                ViewState.ProgressIsSecured = false;
                return;
            }

            var current = s.CurrentLevel();
            var next = s.NextLevel();
            bool secured = s.IsSecured();
            bool atEntry = s.IsAtEntry();

            ViewState.PeriodText = GetPeriodText(s.PeriodKind);
            ViewState.PeriodEndsLabelText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_ENDS));
            ViewState.PeriodEndText = ProfileDateFormat.MonthDay(s.PeriodEnd);
            int daysLeft = Math.Max(0, (int)Math.Ceiling((s.PeriodEnd - DateTime.UtcNow).TotalDays));
            ViewState.DaysLeftText = daysLeft == 0
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_ENDS_TODAY))
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_DAYS_LEFT), daysLeft);

            ViewState.CurrentOrdinal = s.OrdinalOf(s.CurrentRank);
            ViewState.CurrentLevelName = current?.Name ?? s.LevelNameByRank(s.CurrentRank);

            ViewState.ShowScore = s.Mode == LadderMode.Points && s.Score is not null;
            ViewState.ScoreText = s.Score is decimal score ? AchievementValueFormat.Trim(score) : string.Empty;
            ViewState.ScoreUnitText = GetScoreUnitText(s.PeriodKind);

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

            ViewState.ShowBanner = secured && current is not null;
            ViewState.BannerTitleText = current is null
                ? string.Empty
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_SECURED), current.Name);
            ViewState.BannerDetailText = next is null
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_TOP))
                : s.Score is decimal sc && next.Threshold is int t
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BANNER_NEXT), AchievementValueFormat.Trim(Math.Max(0m, t - sc)), next.Name)
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

            ViewState.Levels = s.Levels.Select((level, index) => new UserLadderLevelViewState
            {
                Rank = level.Rank,
                Ordinal = index + 1,
                Name = level.Name,
                IsCurrent = level.Rank == s.CurrentRank,
                IsSatisfied = level.IsSatisfied == true,
                IsSelected = level.Rank == _selectedRank,
                ThresholdText = level.Threshold is int threshold && threshold > 0
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_LEVEL_POINTS), AchievementValueFormat.Trim(threshold))
                    : string.Empty,
            }).ToList();
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
