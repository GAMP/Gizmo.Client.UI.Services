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
            IOptionsMonitor<ClientShopOptions> shopOptions,
            ILogger<HomePageViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            UserProductViewStateLookupService userProductViewStateLookupService,
            AppViewStateLookupService appViewStateLookupService,
            IOptions<PopularItemsOptions> popularItemsOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _shopOptions = shopOptions;
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _appViewStateLookupService = appViewStateLookupService;
            _popularItemsOptions = popularItemsOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptionsMonitor<ClientShopOptions> _shopOptions;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly AppViewStateLookupService _appViewStateLookupService;
        private readonly IOptions<PopularItemsOptions> _popularItemsOptions;
        #endregion

        #region FUNCTIONS

        public async Task RefilterAsync(CancellationToken cancellationToken)
        {
            //only show products if shop is enabled
            if (_shopOptions.CurrentValue.Disabled == false)
            {
                if (_popularItemsOptions.Value.MaxPopularProducts == 0)
                {
                    ViewState.PopularProducts = Enumerable.Empty<UserProductViewState>();
                }
                else
                {
                    var popularProducts = await _gizmoClient.UserPopularProductsGetAsync(new Web.Api.Models.UserPopularProductsFilter()
                    {
                        Limit = _popularItemsOptions.Value.MaxPopularProducts
                    }, cancellationToken);

                    var productIds = popularProducts.Select(a => a.Id).ToList();

                    var products = await _userProductViewStateLookupService.GetStatesAsync(cancellationToken);

                    //filter products
                    products = products.Where(a => productIds.Contains(a.Id)).
                        OrderBy(a => productIds.IndexOf(a.Id));

                    //take desired amount
                    if (_popularItemsOptions.Value.MaxPopularProducts > 0)
                        products = products.Take(_popularItemsOptions.Value.MaxPopularProducts);

                    ViewState.PopularProducts = products
                        .ToList();
                }
            }

            if (_popularItemsOptions.Value.MaxPopularApplications == 0)
            {
                ViewState.PopularApplications = Enumerable.Empty<AppViewState>();
            }
            else
            {
                var popularApplications = await _gizmoClient.UserPopularApplicationsGetAsync(new Web.Api.Models.UserPopularApplicationsFilter()
                {
                    Limit = _popularItemsOptions.Value.MaxPopularApplications
                });

                var applicationIds = popularApplications.Select(a => a.Id).ToList();

                //TODO we might get less apps than expected due to app profile not allowing the app
                var apps = await _appViewStateLookupService.GetFilteredStatesAsync(cancellationToken);

                //filter apps
                apps = apps.Where(a => applicationIds.Contains(a.ApplicationId))
                    .OrderBy(a => applicationIds.IndexOf(a.ApplicationId));

                //take desired amount
                if (_popularItemsOptions.Value.MaxPopularApplications > 0)
                    apps = apps.Take(_popularItemsOptions.Value.MaxPopularApplications);

                ViewState.PopularApplications = apps
                    .ToList();
            }

            DebounceViewStateChanged();
        }

        #endregion

        #region OVERRIDES   

        protected override Task OnInitializing(CancellationToken ct)
        {
            //provide shop settings through the view state
            ViewState.IsShopEnabled = !_shopOptions.CurrentValue.Disabled;

            return base.OnInitializing(ct);
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await RefilterAsync(cancellationToken);
        }

        #endregion
    }
}
