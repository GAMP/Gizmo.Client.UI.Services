using Gizmo.UI.View.States;

using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class AdvertisementsViewState : ViewStateBase
    {
        public IEnumerable<AdvertisementViewState> Advertisements { get; internal set; } = Enumerable.Empty<AdvertisementViewState>();
        public int AdvertisementsCount { get; internal set; }
    }
}
