using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class ShopPageService : ViewStateServiceBase<ShopPageViewState>
    {
        #region CONSTRUCTOR
        public ShopPageService(ShopPageViewState viewState,
            ILogger<ShopPageService> logger,
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;

            _gizmoClient.GetProductsAsync(new ProductsFilter());
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES


        #endregion

        #region FUNCTIONS

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            var productGroups = await _gizmoClient.GetProductGroupsAsync(new ProductGroupsFilter());
            ViewState.ProductGroups = productGroups.Data.Select(a => new ProductGroupViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            var products = await _gizmoClient.GetProductsAsync(new ProductsFilter());
            ViewState.Products = products.Data.Select(a => new ProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                UnitPrice = a.Price,
                UnitPointsAward = a.Points,
                UnitPointsPrice = a.PointsPrice,
                Image = "Cola.png",
                ProductType = a.ProductType
            }).ToList();
        }
    }
}
