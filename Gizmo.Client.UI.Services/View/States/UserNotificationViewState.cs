using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class UserNotificationViewState : ViewStateBase
    {
        #region FIELDS
        private string _title;
        private string _text;
        private UserNotificationState _state;
        private int _notificationId;
        #endregion

        #region PROPERTIES

        public int NotificationId
        {
            get { return _notificationId; }
            internal set { SetProperty(ref _notificationId, value); }
        }

        public string Title
        {
            get { return _title; }
            internal set { SetProperty(ref _title, value); }
        }

        public string Text
        {
            get { return _text; }
            internal set { SetProperty(ref _text, value); }
        }

        public UserNotificationState State
        {
            get { return _state; }
            internal set { SetProperty(ref _state, value); }
        } 
        
        #endregion
    }
}
