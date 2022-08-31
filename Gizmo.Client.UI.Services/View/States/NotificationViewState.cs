using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register(Scope = RegisterScope.Transient)]
    public sealed class NotificationViewState : ViewStateBase
    {
        #region FIELDS
        private int _id;
        private string _time = string.Empty;
        private string _title = string.Empty;
        private string _message = string.Empty;
        #endregion

        #region PROPERTIES

        public int Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        public string Time
        {
            get { return _time; }
            internal set { _time = value; }
        }

        public string Title
        {
            get { return _title; }
            internal set { _title = value; }
        }

        public string Message
        {
            get { return _message; }
            internal set { _message = value; }
        }

        #endregion
    }
}