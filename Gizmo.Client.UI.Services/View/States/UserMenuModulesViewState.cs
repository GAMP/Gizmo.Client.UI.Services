using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public class UserMenuModulesViewState : ViewStateBase
    {
        public List<UserMenuModuleViewState> UserMenuModules { get; internal set; } = new();
    }
}
