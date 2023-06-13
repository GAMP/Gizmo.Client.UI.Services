﻿using System.Linq;
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
            var productStates = await _userProductService.GetFilteredStatesAsync(ViewState.SearchPattern, cToken);
            var productGroups = await _userProductGroupService.GetStatesAsync(cToken);

            //product group look up dictionary
            var productGroupLookup = productGroups.ToDictionary(x => x.ProductGroupId, y => y);

            //sort the products based on product group settings
            var sortedProducts = productStates.OrderBy(x => 
            {
                //try to obtain the product group from lookup
                if (productGroupLookup.TryGetValue(x.ProductGroupId, out var productGroup))
                {
                    switch(productGroup.SortOption)
                    {
                        case ProductSortOptionType.Name:
                            return x.Name;
                        case ProductSortOptionType.Created:
                            return x.CreatedTime.ToString();
                    }
                }

                //by default sort by sort order
                return x.DisplayOrder.ToString();
            });

            //group products 
            var grouppedProducts = ViewState.SelectedUserProductGroupId.HasValue
                ? ViewState.UserGroupedProducts = sortedProducts.Where(x => x.ProductGroupId == ViewState.SelectedUserProductGroupId).GroupBy(x => x.ProductGroupId)
                : ViewState.UserGroupedProducts = sortedProducts.GroupBy(x => x.ProductGroupId);

            //order product groups
            ViewState.UserGroupedProducts = grouppedProducts.OrderBy(x => 
            {
                //try to obtain the product group from lookup
                if (productGroupLookup.TryGetValue(x.Key,out var productGroup))
                {
                    return productGroup.DisplayOrder;
                }

                return x.Key;
            });

            ViewState.RaiseChanged();
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
