using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HomePageViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<UserProductViewState> _popularProducts = Enumerable.Empty<UserProductViewState>();
        private IEnumerable<AppViewState> _popularApplications = Enumerable.Empty<AppViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<UserProductViewState> PopularProducts
        {
            get { return _popularProducts; }
            internal set { _popularProducts = value; }
        }
        
        public IEnumerable<AppViewState> PopularApplications
        {
            get { return _popularApplications; }
            internal set { _popularApplications = value; }
        }

        public bool IsShopEnabled
        {
            get;internal set;
        }

        public bool IsFeedEnabled { get; internal set; }

        #endregion
    }
}
