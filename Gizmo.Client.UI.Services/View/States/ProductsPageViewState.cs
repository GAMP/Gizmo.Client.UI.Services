using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class ProductsPageViewState : ViewStateBase
    {
        public IEnumerable<UserProductGroupViewState> UserProductGroups { get; internal set; } = Enumerable.Empty<UserProductGroupViewState>();

        public IEnumerable<IGrouping<int, UserProductViewState>> UserGroupedProducts { get; internal set; } = Enumerable.Empty<IGrouping<int, UserProductViewState>>();

        public string? SearchPattern { get; internal set; }

        public int? SelectedUserProductGroupId { get; internal set; }
    }
}
