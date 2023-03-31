using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HostUserLockViewState : ViewStateBase
    {
        private bool _isLocked = false;

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { _isLocked = value; }
        }
    }
}
