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
            set { SetProperty(ref _isIdle, value); }
        }

        public TimeOnly IdleTime
        {
            get { return _idleTime; }
            set { SetProperty(ref _idleTime, value); }
        } 
        
        #endregion
    }
}
