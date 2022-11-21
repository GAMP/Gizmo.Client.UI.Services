using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HostViewState : ViewStateBase
    {
        #region FIELDS
        private int _hostNumber = 5;
        private bool _isReserved = true;
        private bool _isLocked = false;
        private bool _isConnected = false;
        #endregion

        #region PROPERTIES

        public int HostNumber
        {
            get { return _hostNumber; }
        }

        public bool IsReserved
        {
            get { return _isReserved; }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { _isLocked = value; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        #endregion
    }
}