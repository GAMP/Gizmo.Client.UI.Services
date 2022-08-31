using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class NotificationsViewState : ViewStateBase
    {
        #region FIELDS
        private List<NotificationViewState> _notifications = new();
        #endregion

        #region PROPERTIES

        public List<NotificationViewState> Notifications
        {
            get { return _notifications; }
            internal set { _notifications = value; }
        }

        #endregion
    }
}
