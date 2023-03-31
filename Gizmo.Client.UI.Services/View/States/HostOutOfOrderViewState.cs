using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class HostOutOfOrderViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isOutOfOrder = false;
        #endregion

        #region PROPERTIES

        public bool IsOutOfOrder
        {
            get { return _isOutOfOrder; }
            internal set { _isOutOfOrder = value; }
        } 
        
        #endregion
    }
}
