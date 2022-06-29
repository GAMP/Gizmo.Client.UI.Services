using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ClientVersionViewState : ViewStateBase
    {
        #region FIELDS
        private string _version = string.Empty;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets client version.
        /// </summary>
        public string Version
        {
            get
            {
                return _version;
            }
            internal set
            {
                SetProperty(ref _version, value);
            }
        } 
        #endregion
    }
}
