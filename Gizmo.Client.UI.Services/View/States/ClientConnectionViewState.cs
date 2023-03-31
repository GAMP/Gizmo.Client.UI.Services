using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ClientConnectionViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isConnected = false;
        private bool _isConnecting = false;
        #endregion

        #region PROPERTIES
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
        #endregion
    }
}
