using Gizmo.UI.View.States;

namespace Gizmo.Client.UI.View.States
{
    public sealed class ConnectionViewState : ViewStateBase
    {
        private bool _isConnected = false;
        private bool _isConnecting = false;

        public bool IsConnected
        {
            get { return _isConnected; }
            internal set { _isConnected = value; }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            internal set { _isConnecting = value; }
        }
    }
}
