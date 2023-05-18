using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.HomeRoute)]
    public sealed class HomePageViewService : ViewStateServiceBase<HomePageViewState>
    {
        #region CONSTRUCTOR
        public HomePageViewService(HomePageViewState viewState,
            ILogger<HomePageViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            UserProductViewStateLookupService userProductViewStateLookupService,
            IOptions<PopularItemsOptions> popularItemsOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _popularItemsOptions = popularItemsOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly IOptions<PopularItemsOptions> _popularItemsOptions;
        #endregion

        #region FUNCTIONS

        public async Task RefilterAsync(CancellationToken cancellationToken)
        {
            var popularProducts = await _gizmoClient.UserPopularProductsGetAsync(new Web.Api.Models.UserPopularProductsFilter()
            {
                Limit = _popularItemsOptions.Value.MaxPopularProducts
            });

            var productIds = popularProducts.Select(a => a.Id).ToList();

            var products = await _userProductViewStateLookupService.GetStatesAsync(cancellationToken);

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
