using Gizmo.UI;
using Gizmo.UI.View.States;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class AdvertisementViewState : ViewStateBase
    {
        public int Id { get; internal set; }
        public bool IsCustomTemplate { get; internal set; }
        public string? Title { get; internal set; }
        public MarkupString Body { get; internal set; }
        public DateTime? StartDate { get; internal set; }
        public DateTime? EndDate { get; internal set; }
        public string? Url { get; internal set; }
        public string? MediaUrl { get; internal set; }
        public AdvertisementMediaUrlType MediaUrlType { get; internal set; }
        public string? ThumbnailUrl { get; internal set; }
        public ViewServiceCommand? Command { get; internal set; }
    }
}
