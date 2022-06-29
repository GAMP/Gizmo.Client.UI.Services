namespace Gizmo.Client.UI.View
{
    /// <summary>
    /// Client connection state.
    /// </summary>
    /// <remarks>
    /// Represents Client real time connection state either by web or normal socket.
    /// </remarks>
    public enum ClientConnectionState
    {
        /// <summary>
        /// Clint is disconnected.
        /// </summary>
        Disconnected=0,
        /// <summary>
        /// Client is connected.
        /// </summary>
        Connected=1,
        /// <summary>
        /// Client is connecting.
        /// </summary>
        Connecting=2,
    }
}
