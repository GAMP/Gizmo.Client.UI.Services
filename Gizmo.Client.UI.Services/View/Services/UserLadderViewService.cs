using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Messaging;
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
            IGizmoClient gizmoClient,
            DebounceActionAsyncService debounceActionService,
            ILogger<UserLadderViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _ladderService = ladderService;
            _standingContext = standingContext;
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _debounceActionService = debounceActionService;
            _debounceActionService.DebounceBufferTime = 500;
        }

        private readonly IUserLadderService _ladderService;
        private readonly IUserLadderStandingContext _standingContext;
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly DebounceActionAsyncService _debounceActionService;
        private UserLadderStanding? _standing;
        private IReadOnlyList<UserLadderTransition> _transitions = Array.Empty<UserLadderTransition>();
        private int? _selectedRank;
        private bool _isOpen;
        private int _refreshPending;

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
            _isOpen = true;
            _selectedRank = null;
            return LoadAsync(cToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _isOpen = false;
            return base.OnNavigatedOut(navigationParameters, cToken);
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
            _standingContext.Changed += OnStandingChanged;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            _standingContext.Changed -= OnStandingChanged;
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            base.OnDisposing(isDisposing);
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            Apply();

            if (ViewState.HasError)
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));

            ViewState.RaiseChanged();
        }

        private void OnStandingChanged(object? sender, EventArgs e)
        {
            if (ViewState.IsLoading)
                return;

            _standing = _standingContext.Standing;

            if (ViewState.HasError && _standing is not null)
            {
                _transitions = _standing.Transitions;
                ViewState.HasError = false;
                ViewState.ErrorMessage = string.Empty;
            }

            Apply();
            DebounceViewStateChanged();
        }

        private void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is not UserAchievementLevelChangedEventMessage)
                return;

            Interlocked.Exchange(ref _refreshPending, 1);
            _debounceActionService.Debounce(RefreshTransitionsAsync);
        }

        private async Task RefreshTransitionsAsync(CancellationToken cToken)
        {
            if (Interlocked.Exchange(ref _refreshPending, 0) == 0 || !_isOpen)
                return;

            if (ViewState.IsLoading)
            {
                Interlocked.Exchange(ref _refreshPending, 1);
                _debounceActionService.Debounce(RefreshTransitionsAsync);
                return;
            }

            try
            {
                _transitions = await _ladderService.GetTransitionsAsync(cToken);
                ApplyHistory();
                DebounceViewStateChanged();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to refresh user ladder transitions after a level change, falling back to the standing's newest transitions.");

                if (_standing is not null)
                {
                    _transitions = _standing.Transitions;
                    ApplyHistory();
                    DebounceViewStateChanged();
                }
            }
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
                ViewState.ProgressIsFull = false;
                ViewState.ShowStatusLine = ViewState.ShowSegments = ViewState.ShowRequirements = ViewState.ShowProgressUpdating = ViewState.ShowKeepWarning = false;
                ViewState.StatusLineText = ViewState.ProgressLabelText = ViewState.ProgressCountText = ViewState.ProgressUnitText = ViewState.RequirementsLabelText = ViewState.ProgressUpdatingText = ViewState.KeepWarningText = string.Empty;
                ViewState.SegmentCount = ViewState.SegmentsLit = 0;
                ViewState.Requirements = Enumerable.Empty<UserLadderRequirementViewState>();
                ViewState.ShowFrozen = ViewState.HasBreakdown = false;
                ViewState.FrozenText = ViewState.BreakdownLabelText = string.Empty;
                ViewState.BreakdownTexts = Array.Empty<string>();
                return;
            }

            var current = s.CurrentLevel();

            ViewState.PeriodText = UserLadderCardText.PeriodText(_localizationService, s.PeriodKind);
            ViewState.PeriodEndsLabelText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_ENDS));
            ViewState.PeriodEndText = ProfileDateFormat.MonthDay(s.PeriodEnd);
            ViewState.DaysLeftText = UserLadderCardText.DaysLeftText(_localizationService, s);

            ViewState.CurrentOrdinal = s.OrdinalOf(s.CurrentRank);
            ViewState.CurrentLevelName = current?.Name ?? s.LevelNameByRank(s.CurrentRank);
            ViewState.CurrentEmblemUrl = current?.EmblemUrl;

            ViewState.ShowScore = UserLadderCardText.ShowScore(s);
            ViewState.ScoreText = UserLadderCardText.ScoreText(s);
            ViewState.ScoreUnitText = UserLadderCardText.ScoreUnitText(_localizationService, s.PeriodKind);

            var breakdownTexts = ViewState.ShowScore
                ? s.Achievements
                    .Where(achievement => achievement.EarnedPoints is > 0m)
                    .OrderByDescending(achievement => achievement.EarnedPoints)
                    .Take(5)
                    .Select(achievement => $"{achievement.Name} · " + _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_LEVEL_POINTS),
                        AchievementValueFormat.Trim(Math.Round(achievement.EarnedPoints!.Value, 1))))
                    .ToList()
                : new List<string>();
            ViewState.HasBreakdown = breakdownTexts.Count > 0;
            ViewState.BreakdownLabelText = ViewState.HasBreakdown
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_BREAKDOWN))
                : string.Empty;
            ViewState.BreakdownTexts = breakdownTexts;

            ViewState.StatusLineText = UserLadderStatusText.RequirementsStatus(_localizationService, s);
            ViewState.ShowStatusLine = ViewState.StatusLineText.Length > 0;

            ViewState.KeepWarningText = UserLadderStatusText.KeepWarning(_localizationService, s);
            ViewState.ShowKeepWarning = ViewState.KeepWarningText.Length > 0;

            ViewState.ProgressUpdatingText = UserLadderCardText.ProgressUpdatingText(_localizationService, s);
            ViewState.ShowProgressUpdating = ViewState.ProgressUpdatingText.Length > 0;

            ViewState.FrozenText = UserLadderCardText.FrozenText(_localizationService, s);
            ViewState.ShowFrozen = ViewState.FrozenText.Length > 0;

            var progress = UserLadderCardText.Progress(_localizationService, s);
            ViewState.ShowProgress = progress.Show;
            ViewState.ProgressPercent = progress.Percent;
            ViewState.ProgressIsFull = progress.IsFull;
            ViewState.ProgressGoalText = progress.GoalText;

            var segments = UserLadderCardText.Segments(_localizationService, s);
            ViewState.ShowSegments = segments.Show;
            ViewState.SegmentCount = segments.Count;
            ViewState.SegmentsLit = segments.Lit;
            ViewState.ProgressLabelText = segments.LabelText;
            ViewState.ProgressCountText = segments.CountText;
            ViewState.ProgressUnitText = segments.UnitText;

            ViewState.ShowBanner = s.Mode != LadderMode.Requirements && s.IsSecured() && current is not null;
            ViewState.BannerTitleText = UserLadderCardText.BannerTitleText(_localizationService, s);
            ViewState.BannerDetailText = UserLadderCardText.BannerDetailText(_localizationService, s);
        }

        private void ApplyLevels()
        {
            var s = _standing;
            if (s is null)
            {
                ViewState.Levels = Enumerable.Empty<UserLadderLevelViewState>();
                return;
            }

            ViewState.Levels = Enumerable.Range(0, s.Levels.Count)
                .Select(index => UserLadderCardText.Level(_localizationService, s, index, _selectedRank))
                .ToList();
        }

        private void ApplyRequirements()
        {
            UserLadderCardText.UserLadderCardRequirements? requirements = _standing is null
                ? null
                : UserLadderCardText.TargetRequirements(_localizationService, _standing);

            ViewState.ShowRequirements = requirements?.Show ?? false;
            ViewState.RequirementsLabelText = requirements?.LabelText ?? string.Empty;
            ViewState.Requirements = requirements?.Rows ?? Enumerable.Empty<UserLadderRequirementViewState>();
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
    }
}
