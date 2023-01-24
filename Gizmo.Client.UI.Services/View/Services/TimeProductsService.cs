using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class TimeProductsService : ViewStateServiceBase<TimeProductsViewState>
    {
        #region CONSTRUCTOR
        public TimeProductsService(TimeProductsViewState viewState,
            ILogger<TimeProductsService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task LoadTimeProductsAsync()
        {
            //TODO: A Load user time products on page loading.

            //Test
            Random random = new Random();

            var timeProducts = Enumerable.Range(1, 18).Select(i => new TimeProductViewState()
            {
                PurchaseDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, random.Next(1, 28)),
                Title = $"Test {i}",
                Time = TimeSpan.FromMinutes(random.Next(3, 180))
            }).ToList();

            ViewState.TimeProducts = timeProducts;
            //End Test

            ViewState.RaiseChanged();
        }

        #endregion
    }
}