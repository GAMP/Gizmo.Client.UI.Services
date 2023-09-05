using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class TimeProductsViewService : ViewStateServiceBase<TimeProductsViewState>
    {
        #region CONSTRUCTOR
        public TimeProductsViewService(TimeProductsViewState viewState,
            ILogger<TimeProductsViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            UserProductViewStateLookupService userProductStateLookupService,
            UserHostGroupViewStateLookupService userHostGroupViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _userProductStateLookupService = userProductStateLookupService;
            _userHostGroupViewStateLookupService = userHostGroupViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly UserProductViewStateLookupService _userProductStateLookupService;
        private readonly UserHostGroupViewStateLookupService _userHostGroupViewStateLookupService;

        private bool _initialized = false;
        private bool _changed = false;
        #endregion

        #region FUNCTIONS

        private async Task<List<TimeProductViewState>> TransformResults(List<UserUsageTimeLevelModel> timeProducts, CancellationToken cToken = default)
        {
            var timeProductsViewStates = new List<TimeProductViewState>();

            foreach (var timeProduct in timeProducts)
            {
                var timeProductViewState = new TimeProductViewState();

                timeProductViewState.TimeProductType = timeProduct.UsageType;

                switch (timeProductViewState.TimeProductType)
                {
                    case UsageType.Rate:

                        if (timeProduct.Rate != null)
                        {
                            timeProductViewState.TimeProductName = _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_RATE_PER_HOUR", timeProduct.Rate.HourlyRate.ToString("C"));
                            timeProductViewState.Source = timeProduct.Rate.InCredit ? _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_CREDIT") : _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_DEPOSIT");
                            timeProductViewState.InCredit = timeProduct.Rate.InCredit;
                        }
                        else
                        {
                            timeProductViewState.TimeProductName = _localizationService.GetString("GIZ_USAGE_TYPE_RATE");
                        }

                        break;

                    case UsageType.TimeFixed:

                        if (timeProduct.TimeFixed != null)
                        {
                            timeProductViewState.TimeProductName = _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_PRODUCT_MINUTES", timeProduct.TimeFixed.TotalMinutes.ToString());

                            var availableMinutesTimeSpan = TimeSpan.FromMinutes(timeProduct.TimeFixed.AvailableMinutes);
                            timeProductViewState.Source = _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_PRODUCT_HOURS_MINUTES", ((int)availableMinutesTimeSpan.TotalHours), availableMinutesTimeSpan.Minutes.ToString().PadLeft(2, '0'));

                            timeProductViewState.PurchaseDate = timeProduct.TimeFixed.PurchaseDate;
                        }
                        else
                        {
                            timeProductViewState.TimeProductName = _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_FIXED_TIME");
                        }

                        break;

                    case UsageType.TimeOffer:

                        if (timeProduct.TimeOffer != null)
                        {
                            var product = await _userProductStateLookupService.GetStateAsync(timeProduct.TimeOffer.ProductId, false, cToken);

                            timeProductViewState.TimeProductName = product.Name;

                            var hostGroups = await _userHostGroupViewStateLookupService.GetStatesAsync(cToken);

                            if (product.ProductType == ProductType.ProductTime && product.TimeProduct.DisallowedHostGroups != null)
                            {
                                timeProductViewState.AvailableHostGroups = hostGroups.Where(a => !product.TimeProduct.DisallowedHostGroups.Contains(a.Id)).Select(a => a.Id);
                            }

                            timeProductViewState.ExpirationDate = timeProduct.TimeOffer.ExpirationTime;
                            timeProductViewState.ProductId = timeProduct.TimeOffer.ProductId;

                            var availableMinutesTimeSpan = TimeSpan.FromMinutes(timeProduct.TimeOffer.AvailableMinutes);
                            timeProductViewState.Source = _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_PRODUCT_HOURS_MINUTES", ((int)availableMinutesTimeSpan.TotalHours), availableMinutesTimeSpan.Minutes.ToString().PadLeft(2, '0'));

                            timeProductViewState.PurchaseDate = timeProduct.TimeOffer.PurchaseDate;
                        }

                        break;
                }

                timeProductViewState.ActivationOrder = timeProduct.ActivationOrder;

                if (timeProduct.AvailableMinutes.HasValue)
                {
                    var availableMinutesTimeSpan = TimeSpan.FromMinutes(timeProduct.AvailableMinutes.Value);
                    timeProductViewState.Time = _localizationService.GetString("GIZ_USER_TIME_PRODUCTS_PRODUCT_HOURS_MINUTES", ((int)availableMinutesTimeSpan.TotalHours), availableMinutesTimeSpan.Minutes.ToString().PadLeft(2, '0'));
                }

                timeProductsViewStates.Add(timeProductViewState);
            }

            return timeProductsViewStates;
        }

        public async Task LoadAsync(CancellationToken cToken = default)
        {
            if (!_initialized || _changed)
            {
                try
                {
                    List<UserUsageTimeLevelModel> timeProductsList = await _gizmoClient.UserUsageTimeLevelsGetAsync(cToken);
                    var userTimeProductsViewStates = await TransformResults(timeProductsList);

                    ViewState.TimeProducts = userTimeProductsViewStates;

                    _initialized = true;
                    _changed = false;

                    DebounceViewStateChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to obtain user time products.");

                    ViewState.SetDefaults();
                }
            }
        }

        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.UserBalanceChange += OnUserBalanceChange;
            _gizmoClient.UsageSessionChange += OnUsageSessionChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.UsageSessionChange -= OnUsageSessionChange;
            _gizmoClient.UserBalanceChange -= OnUserBalanceChange;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        private void OnUsageSessionChange(object? sender, UsageSessionChangeEventArgs e)
        {
            _changed = true;
        }

        private void OnUserBalanceChange(object? sender, UserBalanceEventArgs e)
        {
            _changed = true;
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedOut)
            {
                _initialized = false;
                ViewState.SetDefaults();
            }

            DebounceViewStateChanged();
        }
    }
}
