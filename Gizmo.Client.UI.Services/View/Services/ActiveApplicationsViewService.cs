using System;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Connections.Features;
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
    public sealed class ActiveApplicationsViewService : ViewStateServiceBase<ActiveApplicationsViewState>
    {
        public ActiveApplicationsViewService(ActiveApplicationsViewState viewState,
            IGizmoClient gizmoClient,
            AppExeExecutionViewStateLookupService executionStateLookupService,
            AppExeViewStateLookupService appExeViewStateLookupService,
            ILogger<ActiveApplicationsViewService> logger,
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
            _gizmoClient.ExecutionContextStateChange += OnExecutionContextStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.ExecutionContextStateChange -= OnExecutionContextStateChange;
            base.OnDisposing(isDisposing);
        }

        private async void OnExecutionContextStateChange(object? sender, ClientExecutionContextStateArgs e)
        {
            try
            {
                var activeExecutables = await _appExeExecutionViewStateLookupService.GetStatesAsync();

                var activeExecutablesIds = activeExecutables
                    .Where(x => x.IsRunning || x.IsReady || x.IsFailed || x.IsActive && _gizmoClient.AppCurrentProfilePass(x.AppId))
                    .Select(x => x.AppExeId);

                var executables = await _appExeViewStateLookupService.GetStatesAsync();
                ViewState.Executables = executables.Where(x => activeExecutablesIds.Contains(x.ExecutableId)).ToList();

                DebounceViewStateChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to handle execution context change event.");
            }

            try
            {
                if(e.NewState == ContextExecutionState.Deploying)
                {
                    //notify of deployment if required
                }

                if(e.NewState == ContextExecutionState.Failed)
                {
                    //notify of error
                    await _gizmoClient.NotifyAppExeLaunchFailureAsync(e.ExecutableId, AppExeLaunchFailReason.ExecutableFileNotFound, e.StateObject as Exception);
                }    
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to notify user on app exe launch failure");
            }
        }
    }
}
