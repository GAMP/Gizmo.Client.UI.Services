﻿using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserChangeMobileViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private int _pageIndex;
        private string _mobilePhone = string.Empty;
        private bool _isLoading;
        private string _confirmationCode = string.Empty;
        private bool _isComplete;
        private bool _canResend;
        private TimeSpan _resendTimeLeft;
        #endregion

        #region PROPERTIES

        public int PageIndex
        {
            get { return _pageIndex; }
            internal set { _pageIndex = value; }
        }

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        public string? Prefix { get; internal set; }

        [ValidatingProperty()]
        [Required()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        [ValidatingProperty()]
        public string ConfirmationCode
        {
            get { return _confirmationCode; }
            internal set { SetProperty(ref _confirmationCode, value); }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            internal set { SetProperty(ref _isComplete, value); }
        }

        public bool CanResend
        {
            get { return _canResend; }
            internal set { SetProperty(ref _canResend, value); }
        }

        public TimeSpan ResendTimeLeft
        {
            get { return _resendTimeLeft; }
            internal set { SetProperty(ref _resendTimeLeft, value); }
        }

        #endregion
    }
}
