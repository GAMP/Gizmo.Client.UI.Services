using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class ShopPageViewState : ViewStateBase
    {
        public int? SelectedUserProductGroupId { get; internal set; }

        public IEnumerable<UserProductGroupViewState> UserProductGroups { get; internal set; } = Enumerable.Empty<UserProductGroupViewState>();

        public IEnumerable<IGrouping<string, UserProductViewState>> UserGroupedProducts { get; internal set; } = Enumerable.Empty<IGrouping<string, UserProductViewState>>();
    }
}
