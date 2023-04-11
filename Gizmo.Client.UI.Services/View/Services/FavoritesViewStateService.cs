using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.HomeRoute)]
    [Route(ClientRoutes.ApplicationsRoute)]
    public sealed class FavoritesViewStateService : ViewStateServiceBase<FavoritesViewState>
    {
        #region CONSTRUCTOR
        public FavoritesViewStateService(FavoritesViewState viewState,
            ILogger<FavoritesViewStateService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            AppExeViewStateLookupService appExeViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _appExeViewStateLookupService = appExeViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public async Task RefilterAsync(CancellationToken cancellationToken)
        {
            var popularExecutables = await _gizmoClient.UserPopularExecutablesGetAsync(new Web.Api.Models.UserPopularExecutablesFilter()
            {
                Limit = 10
            });

            var executableIds = popularExecutables.Select(a => a.Id).ToList();

            var exes = await _appExeViewStateLookupService.GetStatesAsync(cancellationToken);

            ViewState.Executables = exes.Where(a => executableIds.Contains(a.ExecutableId)).ToList();

            RaiseViewStateChanged();
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await RefilterAsync(cancellationToken);
        }
    }
}
