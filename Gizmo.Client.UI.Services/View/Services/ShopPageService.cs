using Gizmo.Client.UI.View.States;
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
        private readonly ProductViewStateLookupService _productService;
        private readonly UserProductGroupViewStateLookupService _userProductGroupService;

        public ShopPageService(
            IServiceProvider serviceProvider,
            ILogger<ShopPageService> logger,
            ShopPageViewState viewState,
            ProductViewStateLookupService productService,
            UserProductGroupViewStateLookupService userProductGroupService) : base(viewState, logger, serviceProvider)
        {
            _productService = productService;
            _userProductGroupService = userProductGroupService;
        }

        public async Task SetSelectedProductsGroupIdAsync(int? selectedProductGroupId)
        {
            ViewState.SelectedUserProductGroupId = selectedProductGroupId;

            await SetUserGroupedProductsAsync(selectedProductGroupId);
        }

        private async Task SetUserGroupedProductsAsync(int? selectedProductGroupId, CancellationToken cToken = default)
        {
            var productStates = await _productService.GetStatesAsync(cToken);

            ViewState.UserGroupedProducts = selectedProductGroupId.HasValue
                ? ViewState.UserGroupedProducts = productStates.Where(x => x.ProductGroupId == selectedProductGroupId).GroupBy(x => x.ProductGroupName)
                : ViewState.UserGroupedProducts = productStates.GroupBy(x => x.ProductGroupName);

            ViewState.RaiseChanged();
        }
        private async Task SetUserProductGroupsAsync(CancellationToken cToken = default)
        {
            ViewState.UserProductGroups = await _userProductGroupService.GetStatesAsync(cToken);

            ViewState.RaiseChanged();
        }

        private async void UserProductGroupStatesChanged(object? sender, EventArgs e)
        {
            await SetUserProductGroupsAsync();
        }

        protected override async Task OnNavigatedIn()
        {
            _userProductGroupService.Changed += UserProductGroupStatesChanged;

            await SetUserProductGroupsAsync();
            await SetUserGroupedProductsAsync(ViewState.SelectedUserProductGroupId);
        }
        protected override Task OnNavigatedOut()
        {
            _userProductGroupService.Changed -= UserProductGroupStatesChanged;

            return base.OnNavigatedOut();
        }
    }
}
