using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PurchasesViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<PurchaseViewState> _purchases = Enumerable.Empty<PurchaseViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<PurchaseViewState> Purchases
        {
            get { return _purchases; }
            internal set { _purchases = value; }
        }

        #endregion
    }
}
