using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserCartService : ViewStateServiceBase<UserCartViewState>
    {
        #region CONSTRUCTOR
        public UserCartService(UserCartViewState viewState,
            ILogger<UserCartService> logger,
            IServiceProvider serviceProvider) : base(viewState,logger,serviceProvider)
        {
        }
        #endregion

        #region FIELDS
        private readonly ConcurrentDictionary<int, int> _products = new();
        #endregion

        #region PROPERTIES
        #endregion

        #region FUNCTIONS
        
        public Task AddProductAsyc(int productId, int quantity = 1)
        {
            var productState = ServiceProvider.GetRequiredService<UserCartProductViewState>();
            productState.ProductName = "Some product";
            productState.ProductId = productId;
            return Task.CompletedTask;   
        }

        public Task RemoveProductAsync(int productId, int? quantity)
        {
            return Task.CompletedTask;
        }

        public Task DeleteProduct(int productId)
        {
            if(_products.TryGetValue(productId, out var product))
            {
            }
            return Task.CompletedTask;
        }

        public Task<bool> IsProductInCartAync(int productId)
        {
            return Task.FromResult(_products.ContainsKey(productId));
        }

        public async ValueTask<bool> TryGetProductViewStateAsync(int productId)
        {
            if (_products.TryGetValue(productId, out var productViewState))
                return true;
            await Task.Delay(1000);

            return false;
        }

        #endregion
    }
}
