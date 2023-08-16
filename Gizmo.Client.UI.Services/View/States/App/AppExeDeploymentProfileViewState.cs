using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppExeDeploymentProfileViewState : ViewStateBase
    {
        #region PROPERTIES

        public int DeploymentProfileId { get; internal set; }

        #endregion
    }
}
