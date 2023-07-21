namespace Gizmo.Client.UI.Services.Client
{
    public class DemoAppExeExecutionContext : IAppExeExecutionContext
    {
        private readonly DemoClient _gizmoClient;
        private int _appExeId;
        private ContextExecutionState _currentState = ContextExecutionState.Initial;

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

        private void SetState(ContextExecutionState newState)
        {
            var oldState = _currentState;
            _currentState = newState;

            _gizmoClient.RaiseContextStateChanged(this, new ClientExecutionContextStateArgs(_appExeId, _currentState, oldState));
        }

        public async Task ExecuteAsync(bool reprocess, CancellationToken cancellationToken = default)
        {
            IsExecuting = true;

            SetState(ContextExecutionState.Processing);
            SetState(ContextExecutionState.Validating);
            SetState(ContextExecutionState.ChekingAvailableSpace);
            SetState(ContextExecutionState.Deploying);

            //SetState(ContextExecutionState.Aborting);
            //SetState(ContextExecutionState.Aborted);

            // Simulate task.
            await Task.Delay(3000);

            IsExecuting = false;
            IsAlive = true;
            HasCompleted = true;

            SetState(ContextExecutionState.Starting);
            SetState(ContextExecutionState.ProcessCreated);
            SetState(ContextExecutionState.Completed);
        }

        public async Task TerminateAsync(CancellationToken cancellationToken = default)
        {
            // Simulate task.
            await Task.Delay(3000);

            IsExecuting = false;
            IsAlive = false;
            HasCompleted = false;

            SetState(ContextExecutionState.ProcessExited);
        }

        public Task<bool> TryActivateAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
