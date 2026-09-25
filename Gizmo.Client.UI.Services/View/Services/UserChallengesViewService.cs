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
    [Route(ClientRoutes.UserChallengesRoute)]
    public sealed class UserChallengesViewService : ViewStateServiceBase<UserChallengesViewState>
    {
        public UserChallengesViewService(UserChallengesViewState viewState,
            IUserChallengesService challengesService,
            UserProductViewStateLookupService productLookupService,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            DebounceActionAsyncService debounceActionService,
            ILogger<UserChallengesViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _challengesService = challengesService;
            _productLookupService = productLookupService;
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _debounceActionService = debounceActionService;
            _debounceActionService.DebounceBufferTime = 500;
        }

        private readonly IUserChallengesService _challengesService;
        private readonly UserProductViewStateLookupService _productLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly DebounceActionAsyncService _debounceActionService;
        private IReadOnlyList<UserChallenge> _loaded = Array.Empty<UserChallenge>();
        private IReadOnlyDictionary<int, string> _productNames = new Dictionary<int, string>();
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
                _loaded = await _challengesService.GetChallengesAsync(cToken);
                _productNames = await LoadProductNamesAsync(_loaded, cToken);
                Apply();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load user challenges.");
                _loaded = Array.Empty<UserChallenge>();
                _productNames = new Dictionary<int, string>();
                ViewState.Challenges = Enumerable.Empty<UserChallengeViewState>();
                ViewState.CompletedCountText = string.Empty;
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        private async Task<IReadOnlyDictionary<int, string>> LoadProductNamesAsync(IReadOnlyList<UserChallenge> challenges, CancellationToken cToken)
        {
            var ids = challenges
                .SelectMany(challenge => challenge.Rewards)
                .Where(reward => reward.Kind == ChallengeRewardKind.Product && reward.ProductId is not null)
                .Select(reward => reward.ProductId!.Value)
                .ToHashSet();

            if (ids.Count == 0)
                return new Dictionary<int, string>();

            try
            {
                var products = await _productLookupService.GetStatesAsync(cToken);
                return products
                    .Where(product => ids.Contains(product.Id) && !string.IsNullOrWhiteSpace(product.Name))
                    .ToDictionary(product => product.Id, product => product.Name);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to resolve challenge product reward names.");
                return new Dictionary<int, string>();
            }
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            _isOpen = true;
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
            Apply();

            if (ViewState.HasError)
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));

            ViewState.RaiseChanged();
        }

        private void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is not (UserAchievementChallengeCompletedEventMessage or UserAchievementRewardStatusChangedEventMessage))
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
                _loaded = await _challengesService.GetChallengesAsync(cToken);
                _productNames = await LoadProductNamesAsync(_loaded, cToken);
                Apply();
                ViewState.HasError = false;
                ViewState.ErrorMessage = string.Empty;
                DebounceViewStateChanged();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to refresh user challenges after an achievement event.");
            }
        }

        private void Apply()
        {
            ViewState.Challenges = _loaded.Select(Map).ToList();
            ViewState.CompletedCountText = _localizationService.GetString(
                nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_COMPLETED_COUNT),
                _loaded.Count(challenge => challenge.CompletionsEarned > 0));
        }

        private UserChallengeViewState Map(UserChallenge c)
        {
            bool isDone = c.State == ChallengeState.Done;
            bool isEnded = c.State == ChallengeState.Ended;
            bool hasChip = c.State is not (ChallengeState.Active or ChallengeState.Unknown or ChallengeState.Ended);
            string requirementsCount = c.MetCount is int met
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_POPUP_REQUIREMENTS_COUNT), met, c.Requirements.Count)
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_NOT_COLLECTED));

            return new UserChallengeViewState
            {
                ChallengeId = c.ChallengeId,
                Name = c.Name,
                State = c.State,
                IsDone = isDone,
                IsEnded = isEnded,
                HasChip = hasChip,
                ChipIsSuccess = isDone,
                ChipText = GetChipText(c.State),
                ShowProgressBar = !hasChip,
                ProgressPercent = isDone ? 100m : Math.Clamp(c.Progress ?? 0m, 0m, 100m),
                CountText = c.CompletionsEarned > 0
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_COUNT_PILL), c.CompletionsEarned)
                    : string.Empty,
                WindowText = GetWindowText(c, isEnded, hasChip, popup: false),
                WindowIsWarning = isEnded,
                RequirementsText = c.MetCount is int metCount
                    ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REQUIREMENTS_LINE), metCount, c.Requirements.Count)
                    : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_NOT_COLLECTED)),
                PopupDescription = c.Description?.Trim() ?? string.Empty,
                PopupRequirementsCountText = requirementsCount,
                Requirements = c.Requirements.Select(r => new UserChallengeRequirementViewState
                {
                    AchievementId = r.AchievementId,
                    Name = r.Name,
                    IsMet = r.IsMet,
                }).ToList(),
                Rewards = c.Rewards
                    .Select(r => new UserChallengeRewardViewState
                    {
                        Kind = r.Kind,
                        Text = r.Kind switch
                        {
                            ChallengeRewardKind.Points => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_POINTS), r.Amount),
                            ChallengeRewardKind.Time   => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_TIME), AchievementValueFormat.Duration(r.Amount, _localizationService)),
                            _                          => GetProductRewardText(r),
                        },
                        StatusText = isDone && r.Status is { } status ? GetRewardStatusText(status) : string.Empty,
                    }).ToList(),
                PopupWindowText = GetWindowText(c, isEnded, hasChip, popup: true),
                PopupWindowIsWarning = isEnded,
            };
        }

        private string GetProductRewardText(UserChallengeReward reward)
        {
            string name = reward.ProductId is int id && _productNames.TryGetValue(id, out var productName)
                ? productName
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_PRODUCT_UNKNOWN));

            return reward.Amount > 1
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_PRODUCT), name, reward.Amount)
                : name;
        }

        private string GetWindowText(UserChallenge c, bool isEnded, bool hasChip, bool popup)
        {
            if (c.EndTime is not { } end)
                return string.Empty;

            string date = ProfileDateFormat.MonthDay(end);

            if (isEnded)
                return _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_ENDED), date);

            if (hasChip)
                return string.Empty;

            int daysLeft = Math.Max(0, (int)Math.Ceiling((end - DateTime.UtcNow).TotalDays));

            return popup
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_POPUP_ENDS), date, daysLeft)
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_ENDS), date, daysLeft);
        }

        private string GetChipText(ChallengeState state) => state switch
        {
            ChallengeState.Done          => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_CHIP_COMPLETED)),
            ChallengeState.Paused        => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_PAUSED)),
            ChallengeState.Blocked       => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_BLOCKED)),
            ChallengeState.Unreachable   => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_UNREACHABLE)),
            ChallengeState.Ineligible    => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_INELIGIBLE)),
            ChallengeState.PoolExhausted => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_POOL_EXHAUSTED)),
            ChallengeState.Archived      => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_ARCHIVED)),
            ChallengeState.Unmeasurable  => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_UNMEASURABLE)),
            ChallengeState.NotStarted    => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_STATE_NOT_STARTED)),
            _                            => string.Empty,
        };

        private string GetRewardStatusText(ChallengeRewardStatus status) => status switch
        {
            ChallengeRewardStatus.Pending       => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_STATUS_PENDING)),
            ChallengeRewardStatus.AwaitingClaim => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_STATUS_AWAITING_CLAIM)),
            ChallengeRewardStatus.Delivered     => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_STATUS_DELIVERED)),
            ChallengeRewardStatus.Declined      => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_STATUS_DECLINED)),
            ChallengeRewardStatus.Claimed       => _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CHALLENGES_REWARD_STATUS_CLAIMED)),
            _                                   => string.Empty,
        };
    }
}
