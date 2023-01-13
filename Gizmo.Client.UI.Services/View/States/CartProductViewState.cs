using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class CartProductViewState : ViewStateBase
    {
        #region FIELDS
        private UserCartProductViewState? _userCartProduct;
        #endregion

        #region PROPERTIES

        public UserCartProductViewState? UserCartProduct
        {
            get { return _userCartProduct; }
            internal set { SetProperty(ref _userCartProduct, value); }
        }

        #endregion
    }
}