using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HomePageViewState : ViewStateBase
    {
        #region FIELDS
        private List<ProductViewState> _popularProducts = new();
        #endregion

        #region PROPERTIES

        public List<ProductViewState> PopularProducts
        {
            get { return _popularProducts; }
            internal set { _popularProducts = value; }
        }

        #endregion
    }
}
