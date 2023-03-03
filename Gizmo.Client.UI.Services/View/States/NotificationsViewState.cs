using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class NotificationsViewState : ViewStateBase
    {
        #region FIELDS
        private IEnumerable<NotificationViewState> _notifications = Enumerable.Empty<NotificationViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<NotificationViewState> Notifications
        {
            get { return _notifications; }
            internal set { _notifications = value; }
        }

        #endregion
    }
}
