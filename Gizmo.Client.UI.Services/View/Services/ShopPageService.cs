﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    [Route(ClientRoutes.ShopRoute)]
    public sealed class ShopPageService : ViewStateServiceBase<ShopPageViewState>
    {
        private readonly ProductViewStateLookupService _userProductService;
        private readonly UserProductGroupViewStateLookupService _userProductGroupService;

        public ShopPageService(
            IServiceProvider serviceProvider,
            ILogger<ShopPageService> logger,
            ShopPageViewState viewState,
            ProductViewStateLookupService userProductService,
            UserProductGroupViewStateLookupService userProductGroupService) : base(viewState, logger, serviceProvider)
        {
            _userProductService = userProductService;
            _userProductGroupService = userProductGroupService;
        }

        public async Task UpdateUserGroupedProductsAsync(int? selectedProductGroupId, CancellationToken cToken = default)
        {
            ViewState.SelectedUserProductGroupId = selectedProductGroupId;

            var productStates = await _userProductService.GetStatesAsync(cToken);

            ViewState.UserGroupedProducts = selectedProductGroupId.HasValue
                ? ViewState.UserGroupedProducts = productStates.Where(x => x.ProductGroupId == selectedProductGroupId).GroupBy(x => x.ProductGroupName)
                : ViewState.UserGroupedProducts = productStates.GroupBy(x => x.ProductGroupName);

            ViewState.RaiseChanged();
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

        protected override async Task OnNavigatedIn()
        {
            _userProductService.Changed += UpdateUserGroupedProductsOnChangeAsync;
            _userProductGroupService.Changed += UpdateUserProductGroupsOnChangeAsync;

            await UpdateUserProductGroupsAsync();
            await UpdateUserGroupedProductsAsync(null);
        }
        protected override Task OnNavigatedOut()
        {
            _userProductService.Changed -= UpdateUserGroupedProductsOnChangeAsync;
            _userProductGroupService.Changed -= UpdateUserProductGroupsOnChangeAsync;

            return base.OnNavigatedOut();
        }
    }
}
