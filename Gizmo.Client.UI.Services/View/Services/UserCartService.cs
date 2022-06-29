using Gizmo.Client.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Gizmo.Client.UI.View.Services
{
    public sealed class UserCartService : ViewStateServiceBase<UserCartViewState>
    {
        #region CONSTRUCTOR
        public UserCartService(UserCartViewState viewState,
            IServiceProvider serviceProvider,
            ILogger<UserCartService> logger) : base(viewState,logger)
        {
            _serviceProvider = serviceProvider;
        }
        #endregion

        #region FIELDS
        private readonly ConcurrentDictionary<int, int> _products = new();
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region PROPERTIES
        #endregion

        #region FUNCTIONS
        
        public Task AddProductAsyc(int productId, int quantity = 1)
        {
            var productState = _serviceProvider.GetRequiredService<UserCartProductViewState>();
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

        public override Task IntializeAsync(CancellationToken ct)
        {
            return base.IntializeAsync(ct);
        }
    }
}
