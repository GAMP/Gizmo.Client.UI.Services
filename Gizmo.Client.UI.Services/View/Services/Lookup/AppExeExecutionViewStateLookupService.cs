using System;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class AppExeExecutionViewStateLookupService : ViewStateLookupServiceBase<int, AppExeExecutionViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public AppExeExecutionViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppExeViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var executables = await _gizmoClient.UserExecutablesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in executables.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.AppExeId = item.Id;
                viewState.AppId = item.ApplicationId;

                AddViewState(item.Id, viewState);
            }

            return true;
        }

        protected override async ValueTask<AppExeExecutionViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserExecutableGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.AppExeId = item.Id;
            viewState.AppId = item.ApplicationId;

            return viewState;
        }

        protected override AppExeExecutionViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppExeExecutionViewState>();
            defaultState.AppExeId = lookUpkey;
            return defaultState;
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.ExecutionContextStateChage += OnExecutionContextStateChage;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);

            //remove any attached event handlers
            _gizmoClient.ExecutionContextStateChage -= OnExecutionContextStateChage;
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
                            //PROGRESS_TIMER = new Timer(OnProgressTimerCallback, syncInfo, 0, 200);
                        }
                        viewState.IsIndeterminate = false;
                        break;
                    default:
                        break;
                }

                //raise changed
                viewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process execution context state change, executable id {appExeId}", e.ExecutableId);
            }
        }
    }
}
