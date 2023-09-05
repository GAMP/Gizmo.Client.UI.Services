using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.UserProductsRoute)]
    public sealed class UserTimeProductsPageViewService : ViewStateServiceBase<UserTimeProductsPageViewState>
    {
        #region CONSTRUCTOR
        public UserTimeProductsPageViewService(UserTimeProductsPageViewState viewState,
            ILogger<TimeProductsViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            var timeProductsViewService = ServiceProvider.GetRequiredService<TimeProductsViewService>();
            await timeProductsViewService.LoadAsync(cToken);
        }
    }
}
