using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public class UserMenuModuleViewState : ViewStateBase
    {
        /// <summary>
        /// Gets user menu module metadata.
        /// </summary>
        public UIUserMenuModuleMetadata? MetaData { get; set; }
    }
}
