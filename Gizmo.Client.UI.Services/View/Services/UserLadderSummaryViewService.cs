using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
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
            ILogger<UserLadderSummaryViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _standingContext = standingContext;
            _localizationService = localizationService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly IUserLadderStandingContext _standingContext;
        private readonly ILocalizationService _localizationService;

        public void OpenLadder() => NavigationService.NavigateTo(ClientRoutes.UserLadderRoute);

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _standingContext.Changed += OnStandingChanged;
            _localizationService.LanguageChanged += OnLanguageChanged;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
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
                ViewState.TopBarProgressText = string.Empty;
                return;
            }

            ViewState.Ordinal = s.OrdinalOf(s.CurrentRank);
            ViewState.EmblemUrl = current.EmblemUrl;
            ViewState.LevelName = current.Name;
            ViewState.HeaderStatusText = s.Mode == LadderMode.Requirements
                ? UserLadderStatusText.RequirementsStatus(_localizationService, s)
                : UserLadderStatusText.HeaderPointsStatus(_localizationService, s);
            ViewState.TopBarProgressText = UserLadderStatusText.TopBarProgress(_localizationService, s);
        }
    }
}
