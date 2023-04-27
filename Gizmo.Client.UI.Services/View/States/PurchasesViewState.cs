using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class PurchasesViewState : ViewStateBase
    {
        #region PROPERTIES

        public IEnumerable<UserOrderViewState> Orders { get; internal set; } = Enumerable.Empty<UserOrderViewState>();

        #endregion
    }
}
