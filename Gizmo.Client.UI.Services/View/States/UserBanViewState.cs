using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserBanViewState : ViewStateBase
    {
        #region PROPERTIES

        public bool IsDisabled { get; internal set; }

        public DateTime? EnableDate { get; internal set; }

        public DateTime? DisabledDate { get; internal set; }

        public string? Reason { get; internal set; }

        public TimeSpan Time { get; internal set; }

        #endregion
    }
}
