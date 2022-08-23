using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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

            viewState.ProductGroups = _gizmoClient.GetProductGroups().Select(a => new ProductGroupViewState()
            {
                Id = a.Id,
                Name = a.Name
            }).ToList();

            viewState.Products = _gizmoClient.GetProducts().Select(a => new ProductViewState()
            {
                Id = a.Id,
                ProductGroupId = a.ProductGroupId,
                Name = a.Name,
                Description = a.Description,
                UnitPrice = a.Price,
                UnitPointsAward = a.Points,
                UnitPointsPrice = a.PointsPrice,
                Image = "Cola.png"
            }).ToList();

            _gizmoClient.GetProducts();
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES


        #endregion

        #region FUNCTIONS

        #endregion
    }
}
