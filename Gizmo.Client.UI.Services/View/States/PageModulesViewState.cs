using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PageModulesViewState : ViewStateBase
    {
        public IEnumerable<PageModuleViewState> PageModules { get; internal set; } = Enumerable.Empty<PageModuleViewState>();
    }
}
