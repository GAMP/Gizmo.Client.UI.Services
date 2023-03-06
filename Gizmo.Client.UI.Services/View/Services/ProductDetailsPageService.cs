﻿using System.Web;

using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ProductDetailsRoute)]
    public sealed class ProductDetailsPageService : ViewStateServiceBase<ProductDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ProductDetailsPageService(ProductDetailsPageViewState viewState,
            ILogger<ProductDetailsPageService> logger,
            IServiceProvider serviceProvider,
            UserProductViewStateLookupService productLookupService) : base(viewState, logger, serviceProvider)
        {
            _productLookupService = productLookupService;
        }
        #endregion

        #region FIELDS
        private readonly UserProductViewStateLookupService _productLookupService;
        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? productId = HttpUtility.ParseQueryString(uri.Query).Get("ProductId");
                if (!string.IsNullOrEmpty(productId))
                {
                    if (int.TryParse(productId, out int id))
                    {
                        var productViewState = await _productLookupService.GetStateAsync(id);
                        ViewState.Product = productViewState;
                        DebounceViewStateChange(productViewState);
                    }
                }
            }
        }

        #endregion
    }
}
