using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class PageModuleViewState : ViewStateBase
    {
        /// <summary>
        /// Gets page module metadata.
        /// </summary>
        public UIPageModuleMetadata? MetaData { get; internal set; }
    }
}
