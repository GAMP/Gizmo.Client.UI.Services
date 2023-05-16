using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class FeedImageViewState : ViewStateBase
    {
        /// <summary>
        /// Gets image url.
        /// </summary>
        public string? Url { get; internal set; }
    }
}
