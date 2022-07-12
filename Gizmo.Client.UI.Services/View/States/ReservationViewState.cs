using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class ReservationViewState : ViewStateBase
    {
        #region FIELDS
        private bool _isPending;
        private bool _isActive;
        private DateTime _startTime;
        #endregion

        #region PROPERTIES

        public bool IsPending
        {
            get { return _isPending; }
            set { SetProperty(ref _isPending, value); }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        } 
        
        #endregion
    }
}
