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
            var productStates = await _userProductService.GetFilteredStatesAsync(ViewState.SearchPattern, cToken);
            var productGroups = await _userProductGroupService.GetStatesAsync(cToken);

            var sortedProducts = productGroups.OrderBy(productGroup => productGroup.DisplayOrder)
                .SelectMany(productGroup =>
                {
                    var orderedProducts = productStates.Where(product => product.ProductGroupId == productGroup.ProductGroupId);
                    orderedProducts = productGroup.SortOption switch
                    {
                        ProductSortOptionType.Name => orderedProducts.OrderBy(product => product.Name),
                        ProductSortOptionType.Created => orderedProducts.OrderBy(product => product.CreatedTime),
                        _ => orderedProducts.OrderBy(product => product.DisplayOrder),
                    };
                    return orderedProducts;
                });

            ViewState.UserGroupedProducts = ViewState.SelectedUserProductGroupId.HasValue
                ? ViewState.UserGroupedProducts = sortedProducts.Where(x => x.ProductGroupId == ViewState.SelectedUserProductGroupId).GroupBy(x => x.ProductGroupId)
                : ViewState.UserGroupedProducts = sortedProducts.GroupBy(x => x.ProductGroupId);

            DebounceViewStateChanged();
        }

        public async Task UpdateUserGroupedProductsAsync(int? selectedProductGroupId, CancellationToken cToken = default)
        {
            ViewState.SelectedUserProductGroupId = selectedProductGroupId;

            await RefilterRequest(cToken);
        }

        public async Task UpdateUserProductGroupsAsync(CancellationToken cToken = default)
        {
            var productStates = await _userProductService.GetFilteredStatesAsync(ViewState.SearchPattern, cToken);
            var ids = productStates.Select(a => a.ProductGroupId).Distinct();

            var groupStates = await _userProductGroupService.GetStatesAsync(cToken);

            ViewState.UserProductGroups = groupStates.Where(a => ids.Contains(a.ProductGroupId))
                .OrderBy(a => a.DisplayOrder)
                .ToList();

            DebounceViewStateChanged();
        }

        private async void UpdateUserGroupedProductsOnChangeAsync(object? _, EventArgs __) =>
            await UpdateUserGroupedProductsAsync(ViewState.SelectedUserProductGroupId);
        private async void UpdateUserProductGroupsOnChangeAsync(object? _, EventArgs __) =>
            await UpdateUserProductGroupsAsync();

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
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

            return ClearSearchPattern();
        }

        public async Task ClearSearchPattern()
        {
            ViewState.SearchPattern = string.Empty;
            await RefilterRequest(default);
        }

        public Task NavigateWithSearchAsync(string searchPattern)
        {
            ViewState.SelectedUserProductGroupId = null;
            ViewState.SearchPattern = searchPattern;

            NavigationService.NavigateTo(ClientRoutes.ShopRoute);
            return Task.CompletedTask;
        }
    }
}
