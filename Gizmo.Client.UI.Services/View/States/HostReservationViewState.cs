﻿using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    /// <summary>
    /// Host reservation view state.
    /// </summary>
    [Register()]
    public sealed class HostReservationViewState : ViewStateBase
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
            internal set { _isPending = value; }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            internal set { _startTime = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            internal set { _isActive = value; }
        } 
        
        #endregion
    }
}