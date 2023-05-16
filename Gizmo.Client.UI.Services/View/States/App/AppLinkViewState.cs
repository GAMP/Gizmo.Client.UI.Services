using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AppLinkViewState : ViewStateBase
    {
        #region PROPERTIES

        public int AppLinkId { get; internal set; }

        public int ApplicationId { get; internal set; }

        public string Caption { get; internal set; } = null!;

        public string Description { get; internal set; } = null!;

        public string Url { get; internal set; } = null!;

        public int DisplayOrder { get; internal set; }

        public string? MediaUrl { get; internal set; }

        public AdvertisementMediaUrlType MediaUrlType { get; internal set; }

        public string? ThumbnailUrl { get; internal set; }

        #endregion
    }
}
