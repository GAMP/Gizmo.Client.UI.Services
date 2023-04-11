using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.HomeRoute)]
    public sealed class HomePageViewStateService : ViewStateServiceBase<HomePageViewState>
    {
        #region CONSTRUCTOR
        public HomePageViewStateService(HomePageViewState viewState,
            ILogger<HomePageViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            UserProductViewStateLookupService userProductViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userProductViewStateLookupService = userProductViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public async Task RefilterAsync(CancellationToken cToken)
        {
            var popularProducts = await _gizmoClient.UserPopularProductsGetAsync(new Web.Api.Models.UserPopularProductsFilter()
            {
                Limit = 10
            });

            var productIds = popularProducts.Select(a => a.Id).ToList();

            var products = await _userProductViewStateLookupService.GetStatesAsync(cToken);

            ViewState.PopularProducts = products.Where(a => productIds.Contains(a.Id)).ToList();

            RaiseViewStateChanged();
        }

        #endregion

        #region OVERRIDES       

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await RefilterAsync(cancellationToken);
        }
        #endregion
    }
}
