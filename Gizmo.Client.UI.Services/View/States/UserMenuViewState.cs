using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// User menu view state.
    /// </summary>
    [Register()]
    public sealed class UserMenuViewState : ViewStateBase
    {
        public bool UserOnlineDepositIsVisible { get; internal set; }
        public bool ActiveAppsIsVisible { get; internal set; }
        public bool NotificationsIsVisible { get; internal set; }
        public bool UserLinksIsVisible { get; internal set; }
    }
}
