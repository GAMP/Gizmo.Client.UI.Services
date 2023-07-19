using System.Web;

using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ProductDetailsRoute)]
    public sealed class ProductDetailsPageViewService : ViewStateServiceBase<ProductDetailsPageViewState>
    {
        #region CONSTRUCTOR
        public ProductDetailsPageViewService(ProductDetailsPageViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<ProductDetailsPageViewService> logger,
            IServiceProvider serviceProvider,
            UserProductViewStateLookupService productLookupService,
            IOptions<ClientInterfaceOptions> clientUIOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _productLookupService = productLookupService;
            _clientUIOptions = clientUIOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly UserProductViewStateLookupService _productLookupService;
        private readonly IOptions<ClientInterfaceOptions> _clientUIOptions;
        #endregion

        #region OVERRIDES

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.DisableProductDetails = _clientUIOptions.Value.DisableProductDetails;
            return base.OnInitializing(ct);
        }

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (Uri.TryCreate(NavigationService.GetUri(), UriKind.Absolute, out var uri))
            {
                string? productId = HttpUtility.ParseQueryString(uri.Query).Get("ProductId");
                if (!string.IsNullOrEmpty(productId))
                {
                    if (int.TryParse(productId, out int id))
                    {
                        var productViewState = await _productLookupService.GetStateAsync(id, false, cancellationToken);
                        ViewState.Product = productViewState;

                        //TODO: A DEMO
                        var products = await _productLookupService.GetFilteredStatesAsync(null, cancellationToken);
                        ViewState.RelatedProducts = products.Take(2);
                    }
                }
            }
        }

        public override Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default)
        {
            if (command.Params?.Any() != true)
                return Task.CompletedTask;

            var paramProductId = command.Params.GetValueOrDefault("productId")?.ToString();

            if (paramProductId is null)
                return Task.CompletedTask;

            switch (command.Type)
            {
                case ViewServiceCommandType.Navigate:
                    NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + "?ProductId=" + paramProductId);
                    break;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
