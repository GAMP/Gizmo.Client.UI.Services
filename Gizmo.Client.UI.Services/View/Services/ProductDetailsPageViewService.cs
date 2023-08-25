using System.Globalization;
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
            UserProductViewStateLookupService userProductViewStateLookupService,
            IOptions<ClientInterfaceOptions> clientUIOptions,
            IOptions<ClientShopOptions> shopOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userProductViewStateLookupService = userProductViewStateLookupService;
            _clientUIOptions = clientUIOptions;
            _shopOptions = shopOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly UserProductViewStateLookupService _userProductViewStateLookupService;
        private readonly IOptions<ClientInterfaceOptions> _clientUIOptions;
        private readonly IOptions<ClientShopOptions> _shopOptions;
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
                        var productViewState = await _userProductViewStateLookupService.GetStateAsync(id, false, cancellationToken);
                        ViewState.Product = productViewState;

                        //TODO: A DEMO
                        var products = await _userProductViewStateLookupService.GetFilteredStatesAsync(null, cancellationToken);
                        ViewState.RelatedProducts = products.Take(2);
                    }
                }
            }
        }

        public override bool ValidateCommand<TCommand>(TCommand command)
        {
            if (_shopOptions.Value.Disabled || _clientUIOptions.Value.DisableProductDetails)
                return false;

            if (command.Type != ViewServiceCommandType.Navigate)
                return false;

            if (command.Params?.Any() != true)
                return false;

            var paramProductId = command.Params.GetValueOrDefault("productId")?.ToString();

            if (paramProductId is null)
                return false;

            return true;
        }

        public override async Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default)
        {
            if (_shopOptions.Value.Disabled || _clientUIOptions.Value.DisableProductDetails)
                return;

            if (command.Params?.Any() != true)
                return;

            var paramProductId = command.Params.GetValueOrDefault("productId")?.ToString();

            if (paramProductId is null)
                return;

            var productId = int.Parse(paramProductId, NumberStyles.Number);

            var products = await _userProductViewStateLookupService.GetStatesAsync();
            if (!products.Where(a => a.Id == productId).Any())
            {
                NavigationService.NavigateTo(ClientRoutes.NotFoundRoute);
                return;
            }

            switch (command.Type)
            {
                case ViewServiceCommandType.Navigate:
                    NavigationService.NavigateTo(ClientRoutes.ProductDetailsRoute + "?ProductId=" + paramProductId);
                    break;
            }
        }

        #endregion
    }
}
