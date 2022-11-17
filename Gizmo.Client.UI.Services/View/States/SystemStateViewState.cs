﻿using Gizmo.UI.View.States;
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
            internal set { SetProperty(ref _isLocked, value); }
        }

        public bool IsOutOfOrder
        {
            get { return _isOutOfOrder; }
            internal set { SetProperty(ref _isOutOfOrder, value); }
        }

        public bool IsUserLocked
        {
            get { return _isUserLocked; }
            internal set { SetProperty(ref _isUserLocked, value); }
        } 
        
        #endregion
    }
}
