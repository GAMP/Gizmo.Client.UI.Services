using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Host number view state.
    /// </summary>
    [Register()]
    public sealed class HostNumberViewState : ViewStateBase
    {
        #region FIELDS
        private int _hostNumber = 0;
        #endregion

        #region PROPERTIES
        public int HostNumber
        {
            get { return _hostNumber; }
            internal set
            {
                _hostNumber = value;
            }
        }
        #endregion
    }
}
