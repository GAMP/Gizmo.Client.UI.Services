namespace Gizmo.Client
{
    public class DemoAppExecutionContextResult : IAppExecutionContextResult
    {
        public int AppExeId { get; set; }

        public bool IsSuccess { get; set; }

        public IAppExeExecutionContext? ExecutionContext { get; set; }
    }
}
