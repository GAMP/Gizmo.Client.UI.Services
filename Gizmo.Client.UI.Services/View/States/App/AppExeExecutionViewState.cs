using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExeExecutionViewState : ViewStateBase
    {
        #region PROPERTIES

        public int ExecutableId { get; internal set; }

        public ExecutableState State { get; internal set; }

        public decimal StatePercentage { get; internal set; }

        #endregion
    }
}
