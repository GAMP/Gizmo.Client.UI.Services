using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserLockViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isLocking;
        private bool _isLocked;
        #endregion

        #region PROPERTIES

        public bool IsLocking
        {
            get { return _isLocking; }
            internal set { SetProperty(ref _isLocking, value); }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { SetProperty(ref _isLocked, value); }
        }

        #endregion
    }
}