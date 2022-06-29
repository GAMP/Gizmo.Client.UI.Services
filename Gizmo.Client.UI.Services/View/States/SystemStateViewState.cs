using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class SystemStateViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isLocked;
        private bool _isUserLocked;
        private bool _isOutOfOrder;
        #endregion

        #region PROPERTIES

        public bool IsLocked
        {
            get { return _isLocked; }
            set { SetProperty(ref _isLocked, value); }
        }

        public bool IsOutOfOrder
        {
            get { return _isOutOfOrder; }
            set { SetProperty(ref _isOutOfOrder, value); }
        }

        public bool IsUserLocked
        {
            get { return _isUserLocked; }
            set { SetProperty(ref _isUserLocked, value); }
        } 
        
        #endregion
    }
}
