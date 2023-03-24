using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Active applications view state service.
    /// </summary>
    /// <remarks>
    /// Responsible of filtering out active applications.
    /// </remarks>
    [Register()]
    public sealed class ActiveApplicationsViewStateService : ViewStateServiceBase<ActiveApplicationsViewState>
    {
        public ActiveApplicationsViewStateService(ActiveApplicationsViewState viewState,
            IGizmoClient gizmoClient,
            AppExeExecutionViewStateLookupService executionStateLookupService,
            AppExeViewStateLookupService appExeViewStateLookupService,
            ILogger<ActiveApplicationsViewStateService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _appExeExecutionViewStateLookupService = executionStateLookupService;
            _appExeViewStateLookupService = appExeViewStateLookupService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly AppExeExecutionViewStateLookupService _appExeExecutionViewStateLookupService;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.ExecutionContextStateChage += OneExecutionContextStateChage;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.ExecutionContextStateChage -= OneExecutionContextStateChage;
            base.OnDisposing(isDisposing);           
        } 

        private async void OneExecutionContextStateChage(object? sender, ClientExecutionContextStateArgs e)
        {
            try
            {
                var activeExecutables = await _appExeExecutionViewStateLookupService.GetStatesAsync();

                var activeExecutablesIds = activeExecutables
                    .Where(x => x.IsRunning || x.IsReady || x.IsActive && _gizmoClient.AppCurrentProfilePass(x.AppId))
                    .Select(x => x.AppExeId);

                var executables = await _appExeViewStateLookupService.GetStatesAsync();
                ViewState.Executables = executables.Where(x => activeExecutablesIds.Contains(x.ExecutableId)).ToList();

                DebounceViewStateChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to handle execution context change event.");
            }
        }
    }
}
