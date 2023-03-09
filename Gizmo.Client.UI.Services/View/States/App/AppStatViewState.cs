using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppStatViewState : ViewStateBase
    {
        #region PROPERTIES

        public int ApplicationId { get; internal set; }

        public int TotalExecutions { get; internal set; }

        public int TotalExecutionTime { get; internal set; }

        #endregion
    }
}
