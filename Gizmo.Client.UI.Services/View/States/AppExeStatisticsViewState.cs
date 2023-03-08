using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExeStatisticsViewState : ViewStateBase
    {
        #region PROPERTIES

        public int ExecutableId { get; internal set; }

        public int TotalUserExecutions { get; internal set; }

        public int TotalUserTime { get; internal set; }

        public int TotalExecutions { get; internal set; }

        public int TotalTime { get; internal set; }

        #endregion
    }
}
