using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExeExecutionViewState : ViewStateBase
    {
        #region PROPERTIES

        public int AppExeId { get; internal set; }

        public int AppId { get; internal set; }

        public ExecutableState State { get; internal set; }

        public bool IsActive { get; internal set; }

        public bool IsReady { get; internal set; }

        public bool IsRunning { get; internal set; }

        public decimal Progress { get; internal set; }

        public bool IsIndeterminate { get;internal set; }

        #endregion
    }
}
