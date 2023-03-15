using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HomePageViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<UserProductViewState> _popularProducts = Enumerable.Empty<UserProductViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<UserProductViewState> PopularProducts
        {
            get { return _popularProducts; }
            internal set { _popularProducts = value; }
        }

        public byte CardsColumsCount { get; internal set; } = 8;

        #endregion
    }
}
