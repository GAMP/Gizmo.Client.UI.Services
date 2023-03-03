using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserNotificationsViewState : ViewStateBase
    {
        #region FIELDS
        private readonly IEnumerable<UserNotificationViewState> _notifications = Enumerable.Empty<UserNotificationViewState>();
        #endregion

        #region PROPERTIES

        public IEnumerable<UserNotificationViewState> Notifications
        {
            get { return _notifications; }
        } 
        
        #endregion
    }
}
