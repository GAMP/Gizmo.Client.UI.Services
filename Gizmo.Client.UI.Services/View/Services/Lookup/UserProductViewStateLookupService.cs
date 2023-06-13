using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserProductViewStateLookupService : ViewStateLookupServiceBase<int, UserProductViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly HostGroupViewState _hostGroupViewState;

        public UserProductViewStateLookupService(
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            ILogger<UserProductViewStateLookupService> logger,
            IServiceProvider serviceProvider,
            HostGroupViewState hostGroupViewState) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _hostGroupViewState = hostGroupViewState;
        }

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, UserProductViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<UserProductViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<UserProductViewState> UpdateViewStateAsync(UserProductViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserProductGetAsync(viewState.Id, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override UserProductViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserProductViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default namer";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private void RefreshProductAvailability(UserProductViewState product)
        {
            if (product.PurchaseAvailability != null)
            {
                DateTime? lastTimeRangeEnd = null;

                if (product.PurchaseAvailability.DateRange && product.PurchaseAvailability.EndDate.HasValue && product.PurchaseAvailability.TimeRange)
                {
                    int days = 6;
                    if (product.PurchaseAvailability.StartDate.HasValue)
                    {
                        days = Math.Min((int)product.PurchaseAvailability.EndDate.Value.Subtract(product.PurchaseAvailability.StartDate.Value).TotalDays, days); //TODO: AAA REVIEW
                    }
                    for (int i = 0; i < days; i++)
                    {
                        var date = product.PurchaseAvailability.EndDate.Value.AddDays(i * -1);
                        var lastEndDate = product.PurchaseAvailability.DaysAvailable.Where(a => a.Day == date.DayOfWeek).FirstOrDefault();
                        if (lastEndDate != null && lastEndDate.DayTimesAvailable != null)
                        {
                            var lastEndSecond = lastEndDate.DayTimesAvailable.OrderByDescending(a => a.EndSecond).Select(a => a.EndSecond).FirstOrDefault();
                            TimeSpan endTimeSpan = TimeSpan.FromSeconds(lastEndSecond);
                            lastTimeRangeEnd = new DateTime(date.Year, date.Month, date.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
                            break;
                        }
                    }
                }

                bool expired = (product.PurchaseAvailability.EndDate.HasValue && product.PurchaseAvailability.EndDate.Value.AddDays(1) < DateTime.Now) || (lastTimeRangeEnd.HasValue && lastTimeRangeEnd.Value < DateTime.Now);

                if (product.PurchaseAvailability.DateRange &&
                    ((product.PurchaseAvailability.StartDate.HasValue && product.PurchaseAvailability.StartDate.Value > DateTime.Now) ||
                    expired))
                {
                    if (product.PurchaseAvailability.StartDate.HasValue && product.PurchaseAvailability.StartDate.Value > DateTime.Now)
                    {
                        product.DisallowPurchase = true;
                        product.DisallowPurchaseReason = _localizationService.GetString("GIZ_PRODUCT_NOT_YET_AVAILABLE");
                    }
                    else if (expired)
                    {
                        product.DisallowPurchase = true;
                        product.DisallowPurchaseReason = _localizationService.GetString("GIZ_PRODUCT_NOT_AVAILABLE_ANYMORE");
                    }
                }
                else if (product.PurchaseAvailability.DaysAvailable.Count() > 0)
                {
                    product.DisallowPurchase = true;
                    product.DisallowPurchaseReason = _localizationService.GetString("GIZ_PRODUCT_CURRENTLY_NOT_AVAILABLE");

                    var today = product.PurchaseAvailability.DaysAvailable.Where(a => a.Day == DateTime.Now.DayOfWeek).FirstOrDefault();
                    if (today != null && today.DayTimesAvailable != null)
                    {
                        var timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                        foreach (var time in today.DayTimesAvailable)
                        {
                            if (time.StartSecond <= timeSpan.TotalSeconds && time.EndSecond > timeSpan.TotalSeconds)
                            {
                                product.DisallowPurchase = false;
                                product.DisallowPurchaseReason = string.Empty;
                            }
                        }
                    }
                }
            }

            if (product.ProductType == ProductType.ProductTime)
            {
                if (_hostGroupViewState.HostGroupId.HasValue && product.TimeProduct.DisallowedHostGroups.Contains(_hostGroupViewState.HostGroupId.Value))
                {
                    product.DisallowPurchase = true;
                    product.DisallowPurchaseReason = _localizationService.GetString("GIZ_PRODUCT_TIME_NOT_AVAILABLE_ON_THIS_HOST");
                }

                if (product.TimeProduct.UsageAvailability != null)
                {
                    DateTime? lastTimeRangeEnd = null;

                    if (product.TimeProduct.UsageAvailability.DateRange && product.TimeProduct.UsageAvailability.EndDate.HasValue && product.TimeProduct.UsageAvailability.TimeRange)
                    {
                        int days = 6;
                        if (product.TimeProduct.UsageAvailability.StartDate.HasValue)
                        {
                            days = Math.Min((int)product.TimeProduct.UsageAvailability.EndDate.Value.Subtract(product.TimeProduct.UsageAvailability.StartDate.Value).TotalDays, days); //TODO: AAA REVIEW
                        }
                        for (int i = 0; i < days; i++)
                        {
                            var date = product.TimeProduct.UsageAvailability.EndDate.Value.AddDays(i * -1);
                            var lastEndDate = product.TimeProduct.UsageAvailability.DaysAvailable.Where(a => a.Day == date.DayOfWeek).FirstOrDefault();
                            if (lastEndDate != null && lastEndDate.DayTimesAvailable != null)
                            {
                                var lastEndSecond = lastEndDate.DayTimesAvailable.OrderByDescending(a => a.EndSecond).Select(a => a.EndSecond).FirstOrDefault();
                                TimeSpan endTimeSpan = TimeSpan.FromSeconds(lastEndSecond);
                                lastTimeRangeEnd = new DateTime(date.Year, date.Month, date.Day, endTimeSpan.Hours, endTimeSpan.Minutes, endTimeSpan.Seconds);
                                break;
                            }
                        }
                    }

                    bool expired = (product.TimeProduct.UsageAvailability.EndDate.HasValue && product.TimeProduct.UsageAvailability.EndDate.Value.AddDays(1) < DateTime.Now) || (lastTimeRangeEnd.HasValue && lastTimeRangeEnd.Value < DateTime.Now);

                    if (product.TimeProduct.UsageAvailability.DateRange &&
                        ((product.TimeProduct.UsageAvailability.StartDate.HasValue && product.TimeProduct.UsageAvailability.StartDate.Value > DateTime.Now) ||
                        expired))
                    {
                        if (product.TimeProduct.UsageAvailability.StartDate.HasValue && product.TimeProduct.UsageAvailability.StartDate.Value > DateTime.Now)
                        {
                            product.DisallowUse = true;
                        }
                        else if (expired)
                        {
                            product.DisallowUse = true;
                        }
                    }
                    else if (product.TimeProduct.UsageAvailability.DaysAvailable.Count() > 0)
                    {
                        product.DisallowUse = true;

                        var today = product.TimeProduct.UsageAvailability.DaysAvailable.Where(a => a.Day == DateTime.Now.DayOfWeek).FirstOrDefault();
                        if (today != null && today.DayTimesAvailable != null)
                        {
                            var timeSpan = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                            foreach (var time in today.DayTimesAvailable)
                            {
                                if (time.StartSecond <= timeSpan.TotalSeconds && time.EndSecond > timeSpan.TotalSeconds)
                                {
                                    product.DisallowUse = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private UserProductViewState Map(UserProductModel model, UserProductViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.Name = model.Name;
            result.ProductGroupId = model.ProductGroupId;
            result.Description = model.Description;
            result.ProductType = model.ProductType;
            result.UnitPrice = model.Price;
            result.UnitPointsPrice = model.PointsPrice;
            result.UnitPointsAward = model.PointsAward;
            result.DefaultImageId = model.DefaultImageId;
            result.PurchaseOptions = model.PurchaseOptions;
            result.OrderOptions = model.OrderOptions;

            if (model.PurchaseAvailability != null)
            {
                var hasDateRange = model.PurchaseAvailability.DateRange && (model.PurchaseAvailability.StartDate.HasValue || model.PurchaseAvailability.EndDate.HasValue);
                var hasTimeRange = model.PurchaseAvailability.TimeRange; // && model.PurchaseAvailability.DaysAvailable.Where(a => a.DayTimesAvailable != null && a.DayTimesAvailable.Count() > 0).Count() > 0;

                if (hasDateRange || hasTimeRange)
                {
                    var purchaseAvailability = ServiceProvider.GetRequiredService<ProductAvailabilityViewState>();

                    if (hasDateRange)
                    {
                        purchaseAvailability.DateRange = model.PurchaseAvailability.DateRange;
                        purchaseAvailability.StartDate = model.PurchaseAvailability.StartDate;
                        purchaseAvailability.EndDate = model.PurchaseAvailability.EndDate;
                    }

                    if (hasTimeRange)
                    {
                        purchaseAvailability.TimeRange = model.PurchaseAvailability.TimeRange;

                        var daysAvailable = new List<ProductAvailabilityDayViewState>();

                        if (model.PurchaseAvailability.DaysAvailable != null)
                        {
                            foreach (var day in model.PurchaseAvailability.DaysAvailable)
                            {
                                var dayAvailable = ServiceProvider.GetRequiredService<ProductAvailabilityDayViewState>();

                                dayAvailable.Day = day.Day;

                                if (day.DayTimesAvailable != null)
                                {
                                    var timesAvailable = new List<ProductAvailabilityDayTimeViewState>();

                                    foreach (var time in day.DayTimesAvailable)
                                    {
                                        var timeAvailable = ServiceProvider.GetRequiredService<ProductAvailabilityDayTimeViewState>();

                                        timeAvailable.StartSecond = time.StartSecond;
                                        timeAvailable.EndSecond = time.EndSecond;

                                        timesAvailable.Add(timeAvailable);
                                    }

                                    dayAvailable.DayTimesAvailable = timesAvailable;
                                }

                                daysAvailable.Add(dayAvailable);
                            }
                        }

                        purchaseAvailability.DaysAvailable = daysAvailable;
                    }

                    result.PurchaseAvailability = purchaseAvailability;
                }
            }

            result.IsStockLimited = model.IsStockLimited;
            result.IsRestrictedForGuest = model.IsRestrictedForGuest;
            result.IsRestrictedForUserGroup = model.IsRestrictedForUserGroup;
            result.HiddenHostGroups = model.HiddenHostGroups;

            if (model.ProductType == ProductType.ProductBundle)
            {
                result.BundledProducts = model.Bundle?.BundledProducts.Select(bundle =>
                {
                    var bundledProductResult = ServiceProvider.GetRequiredService<UserProductBundledViewState>();
                    bundledProductResult.Id = bundle.ProductId;
                    bundledProductResult.Quantity = bundle.Quantity;
                    return bundledProductResult;
                }).ToList() ?? Enumerable.Empty<UserProductBundledViewState>();
            }
            else if (model.ProductType == ProductType.ProductTime)
            {
                if (model.TimeProduct != null)
                {
                    var timeProductResult = ServiceProvider.GetRequiredService<UserProductTimeViewState>();

                    timeProductResult.Minutes = model.TimeProduct.Minutes;

                    if (model.TimeProduct.UsageAvailability != null)
                    {
                        var hasDateRange = model.TimeProduct.UsageAvailability.DateRange && (model.TimeProduct.UsageAvailability.StartDate.HasValue || model.TimeProduct.UsageAvailability.EndDate.HasValue);
                        var hasTimeRange = model.TimeProduct.UsageAvailability.TimeRange; // && model.TimeProduct.UsageAvailability.DaysAvailable.Where(a => a.DayTimesAvailable != null && a.DayTimesAvailable.Count() > 0).Count() > 0;

                        if (hasDateRange || hasTimeRange)
                        {
                            var usageAvailability = ServiceProvider.GetRequiredService<ProductAvailabilityViewState>();

                            if (hasDateRange)
                            {
                                usageAvailability.DateRange = model.TimeProduct.UsageAvailability.DateRange;
                                usageAvailability.StartDate = model.TimeProduct.UsageAvailability.StartDate;
                                usageAvailability.EndDate = model.TimeProduct.UsageAvailability.EndDate;
                            }

                            if (hasTimeRange)
                            {
                                usageAvailability.TimeRange = model.TimeProduct.UsageAvailability.TimeRange;

                                var daysAvailable = new List<ProductAvailabilityDayViewState>();

                                if (model.TimeProduct.UsageAvailability.DaysAvailable != null)
                                {
                                    foreach (var day in model.TimeProduct.UsageAvailability.DaysAvailable)
                                    {
                                        var dayAvailable = ServiceProvider.GetRequiredService<ProductAvailabilityDayViewState>();

                                        dayAvailable.Day = day.Day;

                                        if (day.DayTimesAvailable != null)
                                        {
                                            var timesAvailable = new List<ProductAvailabilityDayTimeViewState>();

                                            foreach (var time in day.DayTimesAvailable)
                                            {
                                                var timeAvailable = ServiceProvider.GetRequiredService<ProductAvailabilityDayTimeViewState>();

                                                timeAvailable.StartSecond = time.StartSecond;
                                                timeAvailable.EndSecond = time.EndSecond;

                                                timesAvailable.Add(timeAvailable);
                                            }

                                            dayAvailable.DayTimesAvailable = timesAvailable;
                                        }

                                        daysAvailable.Add(dayAvailable);
                                    }
                                }

                                usageAvailability.DaysAvailable = daysAvailable;
                            }

                            timeProductResult.UsageAvailability = usageAvailability;
                        }
                    }

                    timeProductResult.DisallowedHostGroups = model.TimeProduct.DisallowedHostGroups;
                    timeProductResult.ExpiresAfter = model.TimeProduct.ExpiresAfter;
                    timeProductResult.ExpirationOptions = model.TimeProduct.ExpirationOptions;
                    timeProductResult.ExpireFromOptions = model.TimeProduct.ExpireFromOptions;
                    timeProductResult.ExpireAfterType = model.TimeProduct.ExpireAfterType;
                    timeProductResult.ExpireAtDayTimeMinute = model.TimeProduct.ExpireAtDayTimeMinute;
                    timeProductResult.IsRestrictedForHostGroup = model.TimeProduct.IsRestrictedForHostGroup;

                    result.TimeProduct = timeProductResult;
                }
            }

            //TODO: AAA REFRESH TIMER?
            RefreshProductAvailability(result);

            return result;
        }
        #endregion

        /// <summary>
        /// Gets filtered user product states.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>User product states.</returns>
        /// <remarks>
        /// Only user product states that pass the filters will be returned.
        /// </remarks>
        public async Task<IEnumerable<UserProductViewState>> GetFilteredStatesAsync(string? searchPattern, CancellationToken cancellationToken = default)
        {
            var states = await GetStatesAsync(cancellationToken);

            //only include products allowing client order
            states = states.Where(product => !product.OrderOptions.HasFlag(OrderOptionType.DisallowAllowOrder));

            //Only include products that is allowed for guests.
            states = states.Where(a => !a.IsRestrictedForGuest);

            //only include products that is allowing sale
            states = states.Where(a => !a.OrderOptions.HasFlag(OrderOptionType.RestrictSale));

            //only include products that is allowed for specified user group
            states = states.Where(a => !a.IsRestrictedForUserGroup);

            if (_hostGroupViewState.HostGroupId.HasValue)
            {
                //only include products that is visible for specified host group
                states = states.Where(a => !a.HiddenHostGroups.Contains(_hostGroupViewState.HostGroupId.Value));

                ////Only include time products that are allowed for specified host group.
                //states = states.Where(a => a.ProductType != ProductType.ProductTime || (a.ProductType == ProductType.ProductTime && !a.TimeProduct.DisallowedHostGroups.Contains(_hostGroupViewState.HostGroupId.Value)));
            }

            if (!string.IsNullOrEmpty(searchPattern))
            {
                states = states.Where(a => a.Name.Contains(searchPattern, StringComparison.InvariantCultureIgnoreCase));
            }

            return states.ToList();
        }
    }
}
