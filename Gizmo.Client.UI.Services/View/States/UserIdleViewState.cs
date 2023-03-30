using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserIdleViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isIdle;
        private TimeOnly _idleTime;
        #endregion

        #region PROPERTIES

        public bool IsIdle
        {
            get { return _isIdle; }
            internal set { _isIdle = value; }
        }

        public TimeOnly IdleTime
        {
            get { return _idleTime; }
            internal set { _idleTime = value; }
        } 
        
        #endregion
    }
}
