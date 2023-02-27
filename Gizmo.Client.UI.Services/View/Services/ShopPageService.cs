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
        public void SetSelectedProductGroup(int? selectedProductGroupId) => ViewState.SelectedUserProductGroupId = selectedProductGroupId;

        protected override async Task OnInitializing(CancellationToken cToken)
        {
            await base.OnInitializing(cToken);

            ViewState.UserProductGroups = await _userProductGroupService.GetStatesAsync(cToken);
            ViewState.Products = await _productService.GetStatesAsync(cToken);
        }
    }
}
