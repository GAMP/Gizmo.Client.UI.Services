using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppEnterpriseViewState : ViewStateBase
    {
        #region PROPERTIES

        public int AppEnterpriseId { get; internal set; }

        public string Name { get; internal set; } = null!;

        #endregion
    }
}
