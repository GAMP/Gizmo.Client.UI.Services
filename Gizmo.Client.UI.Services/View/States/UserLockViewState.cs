﻿using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserLockViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _inputPassword = string.Empty;
        private string _lockPassword = string.Empty;
        private bool _isLocking;
        private bool _isLocked;
        private string _error = string.Empty;
        #endregion

        #region PROPERTIES

        public string InputPassword
        {
            get { return _inputPassword; }
            set { SetProperty(ref _inputPassword, value); }
        }

        public string LockPassword
        {
            get { return _lockPassword; }
            internal set { SetProperty(ref _lockPassword, value); }
        }

        public bool IsLocking
        {
            get { return _isLocking; }
            internal set { SetProperty(ref _isLocking, value); }
        }

        public bool IsLocked
        {
            get { return _isLocked; }
            internal set { SetProperty(ref _isLocked, value); }
        }

        public string Error
        {
            get { return _error; }
            internal set { SetProperty(ref _error, value); }
        }

        #endregion
    }
}