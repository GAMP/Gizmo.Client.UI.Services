using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HostLockViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isLocked = true;
        #endregion

        #region PROPERTIES

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { _isLocked = value; }
        } 
        
        #endregion
    }
}
