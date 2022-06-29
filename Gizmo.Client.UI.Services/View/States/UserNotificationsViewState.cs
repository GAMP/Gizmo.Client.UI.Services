using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserNotificationsViewState : ViewStateBase
    {
        #region FIELDS
        private readonly List<UserNotificationViewState> _notifications = new();
        #endregion

        #region PROPERTIES

        public IEnumerable<UserNotificationViewState> Notifications
        {
            get { return _notifications; }
        } 
        
        #endregion
    }
}
