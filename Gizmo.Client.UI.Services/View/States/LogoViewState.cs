using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class LogoViewState : ViewStateBase
    {
        /// <summary>
        /// Gets url.
        /// </summary>
        public string? Logo { get;internal set; }
    }
}
