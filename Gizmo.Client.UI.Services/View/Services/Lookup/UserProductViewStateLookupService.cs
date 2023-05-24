using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserProductViewStateLookupService : ViewStateLookupServiceBase<int, UserProductViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public UserProductViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserProductViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
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

            if (model.PurchaseAvailability != null)
            {
                var purchaseAvailability = ServiceProvider.GetRequiredService<ProductPurchaseAvailabilityViewState>();

                purchaseAvailability.DateRange = model.PurchaseAvailability.DateRange;
                purchaseAvailability.StartDate = model.PurchaseAvailability.StartDate;
                purchaseAvailability.EndDate = model.PurchaseAvailability.EndDate;
                purchaseAvailability.TimeRange = model.PurchaseAvailability.TimeRange;

                var daysAvailable = new List<ProductAvailabilityDayViewState>();

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

                purchaseAvailability.DaysAvailable = daysAvailable;

                result.PurchaseAvailability = purchaseAvailability;
            }

            result.IsStockLimited = model.IsStockLimited;
            result.IsRestrictedForGuest = model.IsRestrictedForGuest;
            result.IsRestrictedForUserGroup = model.IsRestrictedForUserGroup;

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
                        var usageAvailability = ServiceProvider.GetRequiredService<ProductTimeUsageAvailabilityViewState>();

                        usageAvailability.DateRange = model.TimeProduct.UsageAvailability.DateRange;
                        usageAvailability.StartDate = model.TimeProduct.UsageAvailability.StartDate;
                        usageAvailability.EndDate = model.TimeProduct.UsageAvailability.EndDate;
                        usageAvailability.TimeRange = model.TimeProduct.UsageAvailability.TimeRange;

                        var daysAvailable = new List<ProductAvailabilityDayViewState>();

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

                        usageAvailability.DaysAvailable = daysAvailable;

                        timeProductResult.UsageAvailability = usageAvailability;
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

            return result;
        }
        #endregion
    }
}
