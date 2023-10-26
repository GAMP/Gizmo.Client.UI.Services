using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserBalanceViewState : ViewStateBase
    {
        #region PROPERTIES

        [DefaultValue(0)]
        public decimal Balance { get; internal set; }

        [DefaultValue(0)]
        public int PointsBalance { get; internal set; }

        [DefaultValue(0)]
        public decimal Outstanding { get; internal set; }

        public TimeSpan? Time { get; internal set; }

        #endregion
    }
}
