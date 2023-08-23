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

        private async Task<List<TimeProductViewState>> TransformResults(PagedList<UserOrderModel> ordersList, CancellationToken cToken = default)
        {
            var userOrderViewStates = new List<TimeProductViewState>();

            foreach (var order in ordersList.Data)
            {
                var userTimeProductViewState = new TimeProductViewState();

                //TODO: AAA

                userOrderViewStates.Add(userTimeProductViewState);
            }

            return userOrderViewStates;
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

            var ordersList = await _gizmoClient.UserOrdersGetAsync(filters, cToken);
            var userTimeProductsViewStates = await TransformResults(ordersList);

            ViewState.TimeProducts = userTimeProductsViewStates;

            ViewState.PrevCursor = ordersList.PrevCursor;
            ViewState.NextCursor = ordersList.NextCursor;

            ViewState.RaiseChanged();
        }

        public async Task LoadAsync(CancellationToken cToken = default)
        {
            //TODO: AAA
            //Test
            Random random = new Random();

            var timeProducts = Enumerable.Range(1, 18).Select(i => new TimeProductViewState()
            {
                UseOrder = i,
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
