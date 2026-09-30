using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLadderSummaryViewService : ViewStateServiceBase<UserLadderSummaryViewState>
    {
        public UserLadderSummaryViewService(UserLadderSummaryViewState viewState,
            IGizmoClient gizmoClient,
            IUserLadderStandingContext standingContext,
            ILocalizationService localizationService,
            DebounceActionAsyncService debounceActionService,
            ILogger<UserLadderSummaryViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _standingContext = standingContext;
            _localizationService = localizationService;
            _debounceActionService = debounceActionService;
            _debounceActionService.DebounceBufferTime = 500;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly IUserLadderStandingContext _standingContext;
        private readonly ILocalizationService _localizationService;
        private readonly DebounceActionAsyncService _debounceActionService;
        private int _refreshPending;

        public void OpenLadder() => NavigationService.NavigateTo(ClientRoutes.UserLadderRoute);

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;
            _standingContext.Changed += OnStandingChanged;
            _localizationService.LanguageChanged += OnLanguageChanged;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            _standingContext.Changed -= OnStandingChanged;
            _localizationService.LanguageChanged -= OnLanguageChanged;
            base.OnDisposing(isDisposing);
        }

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoginCompleted)
            {
                try
                {
                    await _standingContext.RefreshAsync();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Failed to load ladder standing after login.");
                }
            }
            else if (e.State == LoginState.LoggedOut)
            {
                _standingContext.Clear();
            }
        }

        private void OnStandingChanged(object? sender, EventArgs e)
        {
            Apply();
            DebounceViewStateChanged();
        }

        private void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is not (UserAchievementCompletedEventMessage or UserAchievementLevelChangedEventMessage))
                return;

            Interlocked.Exchange(ref _refreshPending, 1);
            _debounceActionService.Debounce(RefreshStandingAsync);
        }

        private async Task RefreshStandingAsync(CancellationToken cToken)
        {
            if (Interlocked.Exchange(ref _refreshPending, 0) == 0 || !_gizmoClient.IsUserLoggedIn)
                return;

            try
            {
                await _standingContext.RefreshAsync(cToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to refresh ladder standing after an achievement event.");
            }
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            Apply();
            DebounceViewStateChanged();
        }

        private void Apply()
        {
            var s = _standingContext.Standing;
            var current = s?.CurrentLevel();

            ViewState.HasLevel = current is not null;

            if (s is null || current is null)
            {
                ViewState.Ordinal = 0;
                ViewState.EmblemUrl = null;
                ViewState.LevelName = string.Empty;
                ViewState.HeaderStatusText = string.Empty;
                ViewState.ShowTopBarProgress = false;
                ViewState.TopBarProgressPercent = 0m;
                ViewState.TopBarProgressIsFull = false;
                ViewState.PeriodText = ViewState.PeriodEndsLabelText = ViewState.PeriodEndText = ViewState.DaysLeftText = string.Empty;
                ViewState.ShowScore = ViewState.ShowSecuredBadge = ViewState.ShowStatusLine = ViewState.ShowKeepWarning = false;
                ViewState.ScoreText = ViewState.ScoreUnitText = ViewState.StatusLineText = ViewState.KeepWarningText = string.Empty;
                ViewState.ShowProgressUpdating = ViewState.ShowFrozen = false;
                ViewState.ProgressUpdatingText = ViewState.FrozenText = string.Empty;
                ViewState.ShowProgress = ViewState.ProgressIsFull = false;
                ViewState.ProgressTitleText = ViewState.ProgressGoalText = string.Empty;
                ViewState.ProgressPercent = 0m;
                ViewState.ShowSegments = false;
                ViewState.SegmentCount = ViewState.SegmentsLit = 0;
                ViewState.ProgressLabelText = ViewState.ProgressCountText = ViewState.ProgressUnitText = string.Empty;
                ViewState.ShowRequirements = false;
                ViewState.RequirementsLabelText = string.Empty;
                ViewState.Requirements = Enumerable.Empty<UserLadderRequirementViewState>();
                ViewState.ShowBanner = false;
                ViewState.BannerTitleText = ViewState.BannerDetailText = string.Empty;
                ViewState.CurrentLevel = null;
                return;
            }

            ViewState.Ordinal = s.OrdinalOf(s.CurrentRank);
            ViewState.EmblemUrl = current.EmblemUrl;
            ViewState.LevelName = current.Name;
            ViewState.HeaderStatusText = s.Mode == LadderMode.Requirements
                ? UserLadderStatusText.RequirementsStatus(_localizationService, s)
                : UserLadderStatusText.HeaderPointsStatus(_localizationService, s);

            var progress = UserLadderStatusText.TopBarProgressPercent(s);
            ViewState.ShowTopBarProgress = progress is not null;
            ViewState.TopBarProgressPercent = progress ?? 0m;
            ViewState.TopBarProgressIsFull = progress >= 100m;

            ApplyPopover(s);
        }

        private void ApplyPopover(UserLadderStanding s)
        {
            var next = s.NextLevel();
            bool isRequirements = s.Mode == LadderMode.Requirements;
            bool collected = UserLadderStatusText.IsProgressCollected(s);

            ViewState.PeriodText = UserLadderCardText.PeriodText(_localizationService, s.PeriodKind);
            ViewState.PeriodEndsLabelText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PERIOD_ENDS));
            ViewState.PeriodEndText = ProfileDateFormat.MonthDay(s.PeriodEnd);
            ViewState.DaysLeftText = UserLadderCardText.DaysLeftText(_localizationService, s);

            ViewState.ShowScore = UserLadderCardText.ShowScore(s);
            ViewState.ScoreText = UserLadderCardText.ScoreText(s);
            ViewState.ScoreUnitText = UserLadderCardText.ScoreUnitText(_localizationService, s.PeriodKind);

            ViewState.KeepWarningText = UserLadderStatusText.KeepWarning(_localizationService, s);
            ViewState.ShowKeepWarning = ViewState.KeepWarningText.Length > 0;

            ViewState.ShowBanner = isRequirements
                ? next is null && collected && !ViewState.ShowKeepWarning
                : s.IsSecured() && (next is null || UserLadderStatusText.AwaitingStatus(_localizationService, s).Length > 0);
            ViewState.BannerTitleText = UserLadderCardText.BannerTitleText(_localizationService, s);
            ViewState.BannerDetailText = UserLadderCardText.BannerDetailText(_localizationService, s);

            ViewState.ShowSecuredBadge = !ViewState.ShowBanner && collected && (isRequirements
                ? !ViewState.ShowKeepWarning
                : s.IsSecured());

            ViewState.StatusLineText = UserLadderStatusText.RequirementsStatus(_localizationService, s);
            ViewState.ShowStatusLine = ViewState.StatusLineText.Length > 0 && !ViewState.ShowBanner;

            ViewState.ProgressUpdatingText = UserLadderCardText.ProgressUpdatingText(_localizationService, s);
            ViewState.ShowProgressUpdating = ViewState.ProgressUpdatingText.Length > 0;

            ViewState.FrozenText = UserLadderCardText.FrozenText(_localizationService, s);
            ViewState.ShowFrozen = ViewState.FrozenText.Length > 0;

            var points = UserLadderCardText.Progress(_localizationService, s);
            ViewState.ShowProgress = points.Show;
            ViewState.ProgressPercent = points.Percent;
            ViewState.ProgressIsFull = points.IsFull;
            ViewState.ProgressGoalText = points.GoalText;
            ViewState.ProgressTitleText = !points.Show
                ? string.Empty
                : points.GoalIsReach
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROGRESS_TO), next!.Name)
                    : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_LADDER_PROGRESS_TO_RETAIN));

            var segments = UserLadderCardText.Segments(_localizationService, s);
            ViewState.ShowSegments = segments.Show;
            ViewState.SegmentCount = segments.Count;
            ViewState.SegmentsLit = segments.Lit;
            ViewState.ProgressLabelText = segments.LabelText;
            ViewState.ProgressCountText = segments.CountText;
            ViewState.ProgressUnitText = segments.UnitText;

            var requirements = UserLadderCardText.TargetRequirements(_localizationService, s);
            ViewState.ShowRequirements = requirements.Show;
            ViewState.RequirementsLabelText = requirements.LabelText;
            ViewState.Requirements = requirements.Rows;

            ViewState.CurrentLevel = UserLadderCardText.Level(_localizationService, s, s.OrdinalOf(s.CurrentRank) - 1, null);
        }
    }
}
