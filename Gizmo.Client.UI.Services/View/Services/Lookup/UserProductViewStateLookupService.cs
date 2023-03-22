﻿using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserProductViewStateLookupService : ViewStateLookupServiceBase<int, UserProductViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public UserProductViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserProductViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var products = await _gizmoClient.UserProductsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var product in products.Data)
            {
                var viewState = CreateDefaultViewState(product.Id);

                viewState.Id = product.Id;
                viewState.Name = product.Name;

                viewState.ProductGroupId = product.ProductGroupId;
                viewState.Description = product.Description;
                viewState.ProductType = product.ProductType;
                viewState.UnitPrice = product.Price;
                viewState.UnitPointsPrice = product.PointsPrice;
                viewState.DefaultImageId = product.DefaultImageId; 

                if (product.ProductType == ProductType.ProductBundle)
                {
                    viewState.BundledProducts = product.Bundle?.BundledProducts.Select(bundle =>
                    {
                        var bundledProductViewState = ServiceProvider.GetRequiredService<UserProductBundledViewState>();
                        bundledProductViewState.Id = bundle.ProductId;
                        bundledProductViewState.Quantity = bundle.Quantity;
                        return bundledProductViewState;
                    }).ToList() ?? Enumerable.Empty<UserProductBundledViewState>();
                }

                if (product.ProductType == ProductType.ProductTime)
                {
                    if (product.TimeProduct != null)
                    {
                        var timeProductViewState = ServiceProvider.GetRequiredService<UserProductTimeViewState>();
                        timeProductViewState.Minutes = product.TimeProduct.Minutes;
                        viewState.TimeProduct = timeProductViewState;
                    }
                }

                AddOrUpdateViewState(product.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<UserProductViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var product = await _gizmoClient.UserProductGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (product is null)
                return viewState;

            viewState.Id = product.Id;
            viewState.Name = product.Name;

            viewState.ProductGroupId = product.ProductGroupId;
            viewState.Description = product.Description;
            viewState.ProductType = product.ProductType;
            viewState.UnitPrice = product.Price;
            viewState.UnitPointsPrice = product.PointsPrice;

            return viewState;
        }
        protected override UserProductViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserProductViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default namer";

            return defaultState;
        }
    }
}
