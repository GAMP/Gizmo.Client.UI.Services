using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class ShopPageService : ViewStateServiceBase<ShopPageViewState>
    {
        private readonly ProductViewStateLookupService _productService;
        private readonly UserProductGroupViewStateLookupService _userProductGroupService;

        public ShopPageService(
            ShopPageViewState viewState,
            ProductViewStateLookupService productService,
            UserProductGroupViewStateLookupService userProductGroupService,
            ILogger<ShopPageService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _productService = productService;
            _userProductGroupService = userProductGroupService;
        }

        /// <summary>
        /// Select user product group
        /// </summary>
        /// <param name="selectedProductGroupId"></param>
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

        protected override async Task OnInitializing(CancellationToken cToken)
        {
            await base.OnInitializing(cToken);

            _userProductGroupService.Changed += SetUserProductGroupsAsync;

            await SetUserProductGroupsAsync(cToken);
            await SetUserGroupedProductsAsync(ViewState.SelectedUserProductGroupId, cToken);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
        }
    }
}
