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
    public sealed class QuickLaunchService : ViewStateServiceBase<QuickLaunchViewState>
    {
        #region CONSTRUCTOR
        public QuickLaunchService(QuickLaunchViewState viewState,
            ILogger<QuickLaunchService> logger,
            IServiceProvider serviceProvider,
            AppExeViewStateLookupService appExeViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _appExeViewStateLookupService = appExeViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public async Task RefilterAsync(CancellationToken cToken)
        {
            var exes = await _appExeViewStateLookupService.GetStatesAsync(cToken);

            ViewState.Executables = exes.Where(a => a.Options.HasFlag(ExecutableOptionType.QuickLaunch)).ToList();

            ViewState.RaiseChanged();
        }

        #endregion

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cToken = default)
        {
            if(navigationParameters.IsInitial)
                await RefilterAsync(cToken);
        }
    }
}
