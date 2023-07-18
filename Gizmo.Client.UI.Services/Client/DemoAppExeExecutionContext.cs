namespace Gizmo.Client.UI.Services.Client
{
    public class DemoAppExeExecutionContext : IAppExeExecutionContext
    {
        private readonly DemoClient _gizmoClient;
        private int _appExeId;

        public DemoAppExeExecutionContext(DemoClient gizmoClient, int appExeId)
        {
            _gizmoClient = gizmoClient;
            _appExeId = appExeId;
        }

        public bool AutoLaunch { get; set; }

        public bool IsAlive { get; set; }

        public bool IsExecuting { get; set; }

        public bool IsAborting { get; set; }

        public bool HasCompleted { get; set; }

        public Task AbortAsync()
        {
            IsExecuting = false;
            IsAlive = false;
            HasCompleted = false;

            _gizmoClient.RaiseContextStateChanged(this, new ClientExecutionContextStateArgs(_appExeId, ContextExecutionState.Aborted, ContextExecutionState.Started));

            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(bool reprocess, CancellationToken cancellationToken = default)
        {
            IsExecuting = true;

            _gizmoClient.RaiseContextStateChanged(this, new ClientExecutionContextStateArgs(_appExeId, ContextExecutionState.Starting, ContextExecutionState.Initial));

            // Simulate task.
            await Task.Delay(3000);

            IsExecuting = false;
            IsAlive = true;
            HasCompleted = true;

            _gizmoClient.RaiseContextStateChanged(this, new ClientExecutionContextStateArgs(_appExeId, ContextExecutionState.Started, ContextExecutionState.Starting));
        }

        public Task TerminateAsync(CancellationToken cancellationToken = default)
        {
            IsExecuting = false;
            IsAlive = false;
            HasCompleted = false;

            return Task.CompletedTask;
        }

        public Task<bool> TryActivateAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
