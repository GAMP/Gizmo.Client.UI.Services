using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.HomeRoute)]
    public sealed class HomePageService : ViewStateServiceBase<HomePageViewState>
    {
        #region CONSTRUCTOR
        public HomePageService(HomePageViewState viewState,
            ILogger<HomePageService> logger,
            IServiceProvider serviceProvider,
            UserProductViewStateLookupService userProductViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _userProductViewStateLookupService = userProductViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        #endregion

        #region OVERRIDES       

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (navigationParameters.IsInitial)
            {
                ViewState.PopularProducts = (await _userProductViewStateLookupService.GetStatesAsync(cancellationToken)).Take(30);
                ViewStateChanged();
            }
        }
        #endregion
    }
}
