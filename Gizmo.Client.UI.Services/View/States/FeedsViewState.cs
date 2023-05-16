using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Feeds view state.
    /// </summary>
    [Register()]
    public sealed class FeedsViewState : ViewStateBase
    {
        /// <summary>
        /// Gets current feed view state.
        /// </summary>
        public FeedViewState? CurrentFeed
        {
            get;internal set;
        }

        /// <summary>
        /// Gets current feed channel view state.
        /// </summary>
        public FeedChannelViewState? CurrentFeedChannel
        {
            get;internal set;
        }
    }
}
