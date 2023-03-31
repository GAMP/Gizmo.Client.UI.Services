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
        private int _hostNumber = 100;
        private bool _isReserved = true;
        private bool _isLocked = false;
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

        public bool IsReserved
        {
            get { return _isReserved; }
            internal set { _isReserved = value; }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { _isLocked = value; }
        }



        #endregion
    }
}
