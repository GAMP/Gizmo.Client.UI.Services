using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.ApplicationsRoute)]
    public sealed class ApplicationsPageService : ViewStateServiceBase<ApplicationsPageViewState>
    {
        #region CONSTRUCTOR
        public ApplicationsPageService(ApplicationsPageViewState viewState,
            ILogger<ApplicationsPageService> logger,
            IServiceProvider serviceProvider,
            AppViewStateLookupService appViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _appViewStateLookupService = appViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly AppViewStateLookupService _appViewStateLookupService;
        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            ViewState.Applications = await _appViewStateLookupService.GetStatesAsync(cToken);

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
