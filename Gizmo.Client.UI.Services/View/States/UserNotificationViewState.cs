using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserNotificationViewState : ViewStateBase
    {
        #region FIELDS
        private string _title = string.Empty;
        private string _text = string.Empty;
        private UserNotificationState _state;
        private int _notificationId;
        #endregion

        #region PROPERTIES

        public int NotificationId
        {
            get { return _notificationId; }
            internal set { _notificationId = value; }
        }

        public string Title
        {
            get { return _title; }
            internal set { _title = value; }
        }

        public string Text
        {
            get { return _text; }
            internal set { _text = value; }
        }

        public UserNotificationState State
        {
            get { return _state; }
            internal set { _state = value; }
        } 
        
        #endregion
    }
}
