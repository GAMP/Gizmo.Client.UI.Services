using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class FeedChannelViewState : ViewStateBase
    {
        public FeedChannelViewState(FeedImageViewState image) 
        {
            Image = image;
        }

        /// <summary>
        /// Gets feed url.
        /// </summary>
        public string? Url { get; internal set; }

        /// <summary>
        /// Gets description.
        /// </summary>
        public string? Description { get; internal set; }

        /// <summary>
        /// Gets feed changel image.
        /// </summary>
        public FeedImageViewState Image { get; }
    }
}
