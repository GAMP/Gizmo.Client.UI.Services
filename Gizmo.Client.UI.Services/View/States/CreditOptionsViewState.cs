using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class CreditOptionsViewState : ViewStateBase
    {
        #region PROPERTIES

        public CreditType SalesCreditType { get; internal set; }

        public CreditType TimeCreditType { get; internal set; }

        public decimal CreditLimit { get; internal set; }

        public bool IsTimeCreditEnabledByDefault { get; internal set; }

        public bool IsUserTimeCreditEnabled { get; internal set; }

        #endregion
    }
}
