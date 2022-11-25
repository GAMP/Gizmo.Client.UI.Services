using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PurchasesViewState : ViewStateBase
    {
        #region FIELDS
        private List<PurchaseViewState> _purchases = new();
        #endregion

        #region PROPERTIES

        public List<PurchaseViewState> Purchases
        {
            get { return _purchases; }
            internal set { _purchases = value; }
        }

        #endregion
    }
}
