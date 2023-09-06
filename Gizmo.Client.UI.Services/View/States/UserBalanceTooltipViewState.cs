using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserBalanceTooltipViewState : ViewStateBase
    {
        #region PROPERTIES

        public bool DisableShop { get; internal set; }

        public bool DisableOnlineDeposits { get; internal set; }

        #endregion
    }
}
