using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FIELDS
        #endregion

        #region FUNCTIONS

        public Task LoadAsync(CancellationToken cToken = default)
        {
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

            return Task.CompletedTask;
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            await LoadAsync(cToken);
        }
    }
}
