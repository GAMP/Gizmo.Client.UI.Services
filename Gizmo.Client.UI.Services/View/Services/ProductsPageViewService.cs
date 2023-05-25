using System.Linq;
using System.Web;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.ShopRoute)]
    public sealed class ProductsPageViewService : ViewStateServiceBase<ProductsPageViewState>
    {
        private readonly UserProductViewStateLookupService _userProductService;
        private readonly UserProductGroupViewStateLookupService _userProductGroupService;
        private readonly HostGroupViewState _hostGroupViewState;

        public ProductsPageViewService(
            IServiceProvider serviceProvider,
            ILogger<ProductsPageViewService> logger,
            ProductsPageViewState viewState,
            UserProductViewStateLookupService userProductService,
            UserProductGroupViewStateLookupService userProductGroupService,
            HostGroupViewState hostGroupViewState) : base(viewState, logger, serviceProvider)
        {
            _userProductService = userProductService;
            _userProductGroupService = userProductGroupService;
            _hostGroupViewState = hostGroupViewState;
        }

        private async Task RefilterRequest(CancellationToken cToken)
        {
            var productStates = await _userProductService.GetStatesAsync(cToken);

            if (_hostGroupViewState.HostGroupId.HasValue)
            {
                productStates = productStates.Where(a => !a.HiddenHostGroups.Contains(_hostGroupViewState.HostGroupId.Value));
            }

            if (!string.IsNullOrEmpty(ViewState.SearchPattern))
            {
                productStates = productStates.Where(a => a.Name.Contains(ViewState.SearchPattern, StringComparison.InvariantCultureIgnoreCase));
            }

            ViewState.UserGroupedProducts = ViewState.SelectedUserProductGroupId.HasValue
                ? ViewState.UserGroupedProducts = productStates.Where(x => x.ProductGroupId == ViewState.SelectedUserProductGroupId).GroupBy(x => x.ProductGroupId)
                : ViewState.UserGroupedProducts = productStates.GroupBy(x => x.ProductGroupId);

            ViewState.RaiseChanged();
        }

        public async Task UpdateUserGroupedProductsAsync(int? selectedProductGroupId, CancellationToken cToken = default)
        {
            ViewState.SelectedUserProductGroupId = selectedProductGroupId;

            await RefilterRequest(cToken);
        }
        public async Task UpdateUserProductGroupsAsync(CancellationToken cToken = default)
        {
            ViewState.UserProductGroups = await _userProductGroupService.GetStatesAsync(cToken);

            ViewState.RaiseChanged();
        }

        private async void UpdateUserGroupedProductsOnChangeAsync(object? _, EventArgs __) =>
            await UpdateUserGroupedProductsAsync(ViewState.SelectedUserProductGroupId);
        private async void UpdateUserProductGroupsOnChangeAsync(object? _, EventArgs __) =>
            await UpdateUserProductGroupsAsync();

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? searchPattern = HttpUtility.ParseQueryString(uri.Query).Get("SearchPattern");
                if (!string.IsNullOrEmpty(searchPattern))
                {
                    ViewState.SearchPattern = searchPattern;
                }
            }

            _userProductService.Changed += UpdateUserGroupedProductsOnChangeAsync;
            _userProductGroupService.Changed += UpdateUserProductGroupsOnChangeAsync;

            if (navigationParameters.IsInitial)
            {
                await UpdateUserProductGroupsAsync(cToken);
                await UpdateUserGroupedProductsAsync(null, cToken);
            }

            await RefilterRequest(cToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            _userProductService.Changed -= UpdateUserGroupedProductsOnChangeAsync;
            _userProductGroupService.Changed -= UpdateUserProductGroupsOnChangeAsync;

            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        public async Task ClearSearchPattern()
        {
            ViewState.SearchPattern = string.Empty;

            await RefilterRequest(default);

            DebounceViewStateChanged();
        }
    }
}
