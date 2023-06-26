using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class GracePeriodViewState : ViewStateBase
    {
        #region PROPERTIES

        public bool IsInGracePeriod { get; internal set; }

        public TimeSpan Time { get; internal set; }

        #endregion
    }
}
