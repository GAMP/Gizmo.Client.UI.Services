using System;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserProductsRoute)]
    public sealed class TimeProductsViewService : ViewStateServiceBase<TimeProductsViewState>
    {
        #region CONSTRUCTOR
        public TimeProductsViewService(TimeProductsViewState viewState,
            ILogger<TimeProductsViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region FUNCTIONS

        private async Task<List<TimeProductViewState>> TransformResults(PagedList<UserUsageTimeLevelModel> timeProducts, CancellationToken cToken = default)
        {
            var timeProductsViewStates = new List<TimeProductViewState>();

            foreach (var timeProduct in timeProducts.Data)
            {
                var timeProductViewState = new TimeProductViewState();

                timeProductViewState.TimeProductType = timeProduct.UsageType;
                timeProductViewState.TimeProductName = ""; //TODO: AAA NOT EXISTS IN MODEL; ProductId
                timeProductViewState.ActivationOrder = timeProduct.ActivationOrder;
                timeProductViewState.PurchaseDate = DateTime.Now; //TODO: AAA NOT EXISTS IN MODEL; InvoiceLineId
                timeProductViewState.Source = $"Test"; //TODO: AAA NOT EXISTS IN MODEL; InvoiceLineId

                if (timeProduct.AvailableMinutes.HasValue)
                {
                    timeProductViewState.Time = TimeSpan.FromMinutes(timeProduct.AvailableMinutes.Value); //TODO: AAA VERIFY
                }

                timeProductViewState.AvailableHostGroups = Enumerable.Range(1, 10).Select(i => i); //TODO: AAA NOT EXISTS IN MODEL; ProductId

                //TODO: AAA

                timeProductsViewStates.Add(timeProductViewState);
            }

            return timeProductsViewStates;
        }

        public async Task LoadPrevious()
        {
            //if (ViewState.PrevCursor != null)
            //    await LoadCursor(ViewState.PrevCursor, true);
        }

        public async Task LoadNext()
        {
            //if (ViewState.NextCursor != null)
            //    await LoadCursor(ViewState.NextCursor, false);
        }

        public async Task LoadCursor(PaginationCursor? cursor, bool prev, CancellationToken cToken = default)
        {
            var filters = new Web.Api.Models.UserOrdersFilter();

            filters.Pagination.Limit = 8;
            filters.Pagination.SortBy = nameof(Web.Api.Models.UserOrderModel.Date);
            filters.Pagination.IsAsc = false;

            filters.Pagination.Cursor = cursor;

            PagedList<UserUsageTimeLevelModel> timeProductsList = null;// = await _gizmoClient.UserOrdersGetAsync(filters, cToken);
            var userTimeProductsViewStates = await TransformResults(timeProductsList);

            ViewState.TimeProducts = userTimeProductsViewStates;

            ViewState.PrevCursor = timeProductsList.PrevCursor;
            ViewState.NextCursor = timeProductsList.NextCursor;

            ViewState.RaiseChanged();
        }

        public async Task LoadAsync(CancellationToken cToken = default)
        {
            //TODO: AAA
            //Test
            Random random = new Random();

            var timeProducts = Enumerable.Range(1, 18).Select(i => new TimeProductViewState()
            {
                TimeProductType = UsageType.Rate,
                TimeProductName = "Rate",
                ActivationOrder = i,
                PurchaseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, random.Next(1, 28)),
                Source = $"Test {i}",
                Time = TimeSpan.FromMinutes(random.Next(3, 180)),
                AvailableHostGroups = Enumerable.Range(1, 10).Select(i => i)
            }).ToList();

            ViewState.TimeProducts = timeProducts;
            //End Test

            ViewState.RaiseChanged();
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
