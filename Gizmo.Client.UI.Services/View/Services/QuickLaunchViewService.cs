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
    public sealed class QuickLaunchViewService : ViewStateServiceBase<QuickLaunchViewState>
    {
        #region CONSTRUCTOR
        public QuickLaunchViewService(QuickLaunchViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<QuickLaunchViewService> logger,
            IServiceProvider serviceProvider,
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

        public async Task RefilterAsync(CancellationToken cToken)
        {
            var executables = await _appExeViewStateLookupService.GetStatesAsync(cToken);

            //filter out executables
            //must be accessible
            //app must be allowed by current profile
            ViewState.Executables = executables
                .Where(appExe => appExe.Options.HasFlag(ExecutableOptionType.QuickLaunch))
                .Where(appExe => appExe.Accessible)
                .Where(appExe => _gizmoClient.AppCurrentProfilePass(appExe.ApplicationId))               
                .ToList();

            ViewState.RaiseChanged();
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            if (navigationParameters.IsInitial)
                await RefilterAsync(cToken);
        }
    }
}
