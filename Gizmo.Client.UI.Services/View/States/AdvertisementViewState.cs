using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AdvertisementViewState : ViewStateBase
    {
        public int Id { get; internal set; }
        public string? Title { get; internal set; }
        public string Body { get; internal set; } = null!;
        public DateTime? StartDate { get; internal set; }
        public DateTime? EndDate { get; internal set; }
        public string? Url { get; internal set; }
        public string? MediaUrl { get; internal set; }
        public string ThumbnailUrl { get; internal set; } = null!;
        public AdvertisementThumbnailType ThumbnailType { get; internal set; }
        public bool IsCustomTemplate { get; internal set; }
        public AdvertisementCommand? Command { get; internal set; }
    }

    public sealed class AdvertisementCommand
    {
        public int PathId { get; internal set; }
        public string[] Parts { get; internal set; } = Array.Empty<string>();
        public AdvertisementCommandType CommandType { get; internal set; }
    }
}
