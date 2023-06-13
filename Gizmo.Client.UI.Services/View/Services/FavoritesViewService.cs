using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.HomeRoute)]
    [Route(ClientRoutes.ApplicationsRoute)]
    public sealed class FavoritesViewService : ViewStateServiceBase<FavoritesViewState>
    {
        #region CONSTRUCTOR
        public FavoritesViewService(FavoritesViewState viewState,
            ILogger<FavoritesViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            AppExeViewStateLookupService appExeViewStateLookupService,
            IOptions<PopularItemsOptions> popularItemsOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _appExeViewStateLookupService = appExeViewStateLookupService;
            _popularItemsOptions = popularItemsOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        private readonly IOptions<PopularItemsOptions> _popularItemsOptions;
        #endregion

        #region FUNCTIONS

        public async Task RefilterAsync(CancellationToken cancellationToken)
        {
            var popularExecutables = await _gizmoClient.UserPopularExecutablesGetAsync(new Web.Api.Models.UserPopularExecutablesFilter()
            {
                Limit = _popularItemsOptions.Value.MaxQuickLaunchExecutables,
                CurrentUserOnly = true
            });

            var executableIds = popularExecutables
                .Select(a => a.Id)
                .ToList();

            var executables = await _appExeViewStateLookupService.GetStatesAsync(cancellationToken);

            //filter out executables
            //must be accessible
            //app must be allowed by current profile
            ViewState.Executables = executables
                .Where(appExe => appExe.Accessible)
                .Where(appExe => _gizmoClient.AppCurrentProfilePass(appExe.ApplicationId))
                .Where(appExe => executableIds.Contains(appExe.ExecutableId))
                .ToList();

            RaiseViewStateChanged();
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await RefilterAsync(cancellationToken);
        }
    }
}
