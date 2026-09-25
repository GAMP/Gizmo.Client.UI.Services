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
        }
    }
}
