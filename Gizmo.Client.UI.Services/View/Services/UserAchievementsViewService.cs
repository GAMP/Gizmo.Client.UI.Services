using System.Web;
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
    [Route(ClientRoutes.UserAchievementsRoute)]
    public sealed class UserAchievementsViewService : ViewStateServiceBase<UserAchievementsViewState>
    {
        public UserAchievementsViewService(UserAchievementsViewState viewState,
            IUserAchievementsService achievementsService,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            DebounceActionAsyncService debounceActionService,
            ILogger<UserAchievementsViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _achievementsService = achievementsService;
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _debounceActionService = debounceActionService;
            _debounceActionService.DebounceBufferTime = 500;
        }

        private readonly IUserAchievementsService _achievementsService;
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly DebounceActionAsyncService _debounceActionService;
        private IReadOnlyList<UserAchievement> _loaded = Array.Empty<UserAchievement>();
        private int? _highlightedId;
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
                _loaded = await _achievementsService.GetAchievementsAsync(cToken);
                ViewState.Achievements = _loaded.Select(Map).ToList();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load user achievements.");
                _loaded = Array.Empty<UserAchievement>();
                ViewState.Achievements = Enumerable.Empty<UserAchievementViewState>();
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _isOpen = true;
            _highlightedId = null;
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri)
                && int.TryParse(HttpUtility.ParseQueryString(uri.Query).Get("AchievementId"), out int achievementId))
            {
                _highlightedId = achievementId;
            }

            foreach (var item in ViewState.Achievements)
                item.IsHighlighted = false;

            return LoadAsync(cToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _isOpen = false;
            return base.OnNavigatedOut(navigationParameters, cToken);
        }

        public void Highlight(int achievementId)
        {
            if (_highlightedId == achievementId)
                return;

            _highlightedId = achievementId;

            foreach (var item in ViewState.Achievements)
                item.IsHighlighted = item.AchievementId == achievementId;

            ViewState.RaiseChanged();
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _localizationService.LanguageChanged += OnLanguageChanged;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _localizationService.LanguageChanged -= OnLanguageChanged;
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            base.OnDisposing(isDisposing);
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            ViewState.Achievements = _loaded.Select(Map).ToList();

            if (ViewState.HasError)
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));

            ViewState.RaiseChanged();
        }

        private void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is not UserAchievementCompletedEventMessage)
                return;

            Interlocked.Exchange(ref _refreshPending, 1);
            _debounceActionService.Debounce(RefreshAsync);
        }

        private async Task RefreshAsync(CancellationToken cToken)
        {
            if (Interlocked.Exchange(ref _refreshPending, 0) == 0 || !_isOpen)
                return;

            if (ViewState.IsLoading)
            {
                Interlocked.Exchange(ref _refreshPending, 1);
                _debounceActionService.Debounce(RefreshAsync);
                return;
            }

            try
            {
                _loaded = await _achievementsService.GetAchievementsAsync(cToken);
                ViewState.Achievements = _loaded.Select(Map).ToList();
                ViewState.HasError = false;
                ViewState.ErrorMessage = string.Empty;
                DebounceViewStateChanged();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to refresh user achievements after an achievement event.");
            }
        }

        private UserAchievementViewState Map(UserAchievement a)
        {
            string rangeWord = GetRangeWord(a.Range);
            bool isEarned = a.State == AchievementState.Earned;
            bool hasChip = a.State is AchievementState.Paused or AchievementState.Archived or AchievementState.Unmeasurable;
            bool popupIsCompleted = a.State is AchievementState.Paused or AchievementState.Archived;
            string standing = GetStandingText(a, isEarned, rangeWord);

            return new UserAchievementViewState
            {
                AchievementId = a.AchievementId,
                Name = a.Name,
                ImageUrl = a.ImageUrl,
                ImageIsSvg = a.ImageIsSvg,
                State = a.State,
                TotalCompletions = a.TotalCompletions,
                IsEarned = isEarned,
                HasChip = hasChip,
                ShowProgressBar = !hasChip,
                ProgressPercent = isEarned ? 100m : Math.Clamp(a.Progress ?? 0m, 0m, 100m),
                CountText = a.TotalCompletions > 0
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_COUNT_PILL), a.TotalCompletions)
                    : string.Empty,
                StandingText = standing,
                ChipText = a.State switch
                {
                    AchievementState.Paused or AchievementState.Archived => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_CHIP_COMPLETED)),
                    AchievementState.Unmeasurable => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_CHIP_UNAVAILABLE)),
                    _ => string.Empty,
                },
                PopupIsCompleted = popupIsCompleted,
                PopupDescription = string.IsNullOrWhiteSpace(a.Description) ? a.Name : a.Description,
                PopupProgressText = standing,
                PopupResetText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_RESETS),
                    ProfileDateFormat.MonthDay(a.InstanceEnd)),
                PopupCompletedText = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_EARNED_FOR_LIFE)),
                IsHighlighted = a.AchievementId == _highlightedId,
            };
        }

        private string GetStandingText(UserAchievement a, bool isEarned, string rangeWord)
        {
            if (isEarned)
                return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_INSTANCE_EARNED), a.InstanceCompletions, a.MaxCompletionsPerRange, rangeWord);

            if (a.Progress is not { } progress)
                return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_NOT_COLLECTED));

            decimal remainder = progress / 100m * a.TargetValue;
            string pair = AchievementValueFormat.Pair(remainder, a.TargetValue, a.Unit, _localizationService);

            string? unitLabel = a.Unit switch
            {
                AchievementSignalUnit.Points => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_UNIT_POINTS)),
                AchievementSignalUnit.Days => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_UNIT_DAYS)),
                _ => null,
            };

            if (!string.IsNullOrEmpty(unitLabel))
                pair += $" {unitLabel}";

            return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_PROGRESS_LINE), pair, rangeWord);
        }

        private string GetRangeWord(AchievementPeriod range) => range switch
        {
            AchievementPeriod.Day => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_RANGE_DAY)),
            AchievementPeriod.Week => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_RANGE_WEEK)),
            AchievementPeriod.Month => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_RANGE_MONTH)),
            AchievementPeriod.Quarter => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_RANGE_QUARTER)),
            _ => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_ACHIEVEMENTS_RANGE_YEAR)),
        };
    }
}
