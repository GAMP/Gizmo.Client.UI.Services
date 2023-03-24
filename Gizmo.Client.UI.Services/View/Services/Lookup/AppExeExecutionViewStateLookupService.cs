using System.Collections.Concurrent;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppExeExecutionViewStateLookupService : ViewStateLookupServiceBase<int, AppExeExecutionViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        private readonly ConcurrentDictionary<int, IAppExecutionContextSyncInfo> _appExecutionContextSyncInfo = new();
        private Timer? _syncUpdateTimer;
        private readonly object _syncUpdateCallbackLock = new();
        private readonly int _syncUpdaterTimerTime = 1000;

        public AppExeExecutionViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppExeViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDE FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _syncUpdateTimer?.Dispose();
            _syncUpdateTimer = new Timer(SyncUpdateTimerCallback, null, _syncUpdaterTimerTime, _syncUpdaterTimerTime);

            _gizmoClient.ExecutionContextStateChage += OnExecutionContextStateChage;

            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);

            _syncUpdateTimer?.Dispose();
            _syncUpdateTimer = null;

            //remove any attached event handlers
            _gizmoClient.ExecutionContextStateChage -= OnExecutionContextStateChage;
        }
        
        protected override async Task<IDictionary<int, AppExeExecutionViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserExecutablesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppExeExecutionViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserExecutableGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppExeExecutionViewState> UpdateViewStateAsync(AppExeExecutionViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserExecutableGetAsync(viewState.AppExeId, cToken);
            
            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override AppExeExecutionViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppExeExecutionViewState>();

            defaultState.AppExeId = lookUpkey;
            
            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AppExeExecutionViewState Map(UserExecutableModel model, AppExeExecutionViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            result.AppId = model.ApplicationId;
            
            return result;
        }
        private async void OnExecutionContextStateChage(object? sender, ClientExecutionContextStateArgs e)
        {
            //filter out states that not of an interest to us
            switch (e.NewState)
            {
                case ContextExecutionState.ProcessExited:
                case ContextExecutionState.Finalized:
                case ContextExecutionState.Initial:
                case ContextExecutionState.Failed:
                    return;
            }

            try
            {
                //check if correct context type is supplied by the sender
                if (sender is not IAppExeExecutionContext context)
                    return;

                //get associated view state
                var viewState = await GetStateAsync(e.ExecutableId);

                viewState.IsRunning = context.IsAlive;
                viewState.IsActive = context.IsExecuting;
                viewState.IsReady = context.HasCompleted || e.NewState == ContextExecutionState.Completed && !context.IsExecuting;

                //update progress values
                if (context.IsExecuting)
                {
                    viewState.IsIndeterminate = true;
                    viewState.Progress = 0;
                }
                else
                {
                    viewState.IsIndeterminate = false;
                }

                //deployment progress will only be reaised once
                //we can stop trackin executable file synchronization on any state change
                _appExecutionContextSyncInfo.Remove(e.ExecutableId, out var _);

                switch (e.NewState)
                {
                    case ContextExecutionState.Released:
                    case ContextExecutionState.Destroyed:
                        viewState.IsReady = false;
                        viewState.IsActive = false;
                        break;
                    case ContextExecutionState.Deploying:
                        if (e.StateObject is IAppExecutionContextSyncInfo syncInfo)
                        {
                            _appExecutionContextSyncInfo.AddOrUpdate(e.ExecutableId, syncInfo, (k, v) => syncInfo);
                        }

                        //once sync starts we should be able to determine progress
                        viewState.IsIndeterminate = false;
                        break;
                    default:
                        break;
                }

                //raise changed
                DebounceViewStateChange(viewState);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process execution context state change, executable id {appExeId}", e.ExecutableId);
            }
        }
        private void SyncUpdateTimerCallback(object? state)
        {
            //nothing to do here if there are no synchronizations to track
            if (_appExecutionContextSyncInfo.IsEmpty)
                return;

            //since the timer might hit multiple times we need to have a lock
            //and allow only single update routine to run
            if (Monitor.TryEnter(_syncUpdateCallbackLock))
            {
                try
                {
                    foreach (var appExe in _appExecutionContextSyncInfo)
                    {
                        if (TryGetState(appExe.Key, out var viewState))
                        {
                            var syncer = appExe.Value;

                            long total = syncer.Total;
                            long written = syncer.TotalWritten;

                            if (total > 0)
                            {
                                viewState.Progress = written * 100 / total;
                                if (viewState.IsIndeterminate)
                                    viewState.IsIndeterminate = false;
                            }
                            else
                            {
                                if (!viewState.IsIndeterminate)
                                    viewState.IsIndeterminate = true;
                                viewState.Progress = 0;
                            }

                            DebounceViewStateChange(viewState);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed updating execution context view state synchronization progress.");
                }
                finally
                {
                    Monitor.Exit(_syncUpdateCallbackLock);
                }
            }
        }
        #endregion
    }
}
