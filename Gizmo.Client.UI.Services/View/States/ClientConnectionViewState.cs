using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ClientConnectionViewState : ViewStateBase
    {
        #region FIELDS
        private ClientConnectionState _connectionState = ClientConnectionState.Disconnected;
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets client connection state.
        /// </summary>
        public ClientConnectionState ConnectionState
        {
            get
            {
                return _connectionState;
            }
            internal set
            {
                SetProperty(ref _connectionState, value);
            }
        } 

        #endregion
    }
}
