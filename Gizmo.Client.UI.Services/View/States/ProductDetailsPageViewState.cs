using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ProductDetailsPageViewState : ViewStateBase
    {
        #region FIELDS
        private UserProductViewState _product = new();
        private IEnumerable<UserProductViewState> _relatedProducts = Enumerable.Empty<UserProductViewState>();
        #endregion

        #region PROPERTIES

        public UserProductViewState Product
        {
            get { return _product; }
            internal set { _product = value; }
        }

        public IEnumerable<UserProductViewState> RelatedProducts
        {
            get { return _relatedProducts; }
            internal set { _relatedProducts = value; }
        }

        #endregion
    }
}
