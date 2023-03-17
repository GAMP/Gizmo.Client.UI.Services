namespace Gizmo.Client.UI.View
{
    //TODO: A
    public enum ExecutableState
    {
        None,
        Loading, //IsActive
        Deployment, //IsReady
        Running, //IsRunning
        Terminating,
        Stopped
    }
}
