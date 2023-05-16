using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class FeedViewState : ViewStateBase
    {
        public FeedViewState(FeedImageViewState image, FeedLinkViewState link)
        {
            Image = image;
            Link = link;
        }

        /// <summary>
        /// Gets feed title.
        /// </summary>
        public string Title { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets feed summary.
        /// </summary>
        public string Summary
        {
            get; internal set;
        } = string.Empty;

        /// <summary>
        /// Gets feed publish date.
        /// </summary>
        public DateTime? PublishDate { get; internal set; }

        /// <summary>
        /// Gets feed description.
        /// </summary>
        /// <remarks>
        /// RSS specs <see href="https://en.wikipedia.org/wiki/RSS"/>
        /// </remarks>
        public string? Content
        {
            get; internal set;
        }

        /// <summary>
        /// Gets feed default image.
        /// </summary>
        public FeedImageViewState Image
        {
            get;
        }

        /// <summary>
        /// Gets feed default link.
        /// </summary>
        public FeedLinkViewState Link
        {
            get;
        }
    }
}
