using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserChangeEmailViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private int _pageIndex;
        private string _email = string.Empty;
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
        [Required(ErrorMessageResourceType = typeof(Resources.Properties.Resources), ErrorMessageResourceName = "GIZ_GEN_VE_REQUIRED_FIELD")]
        public string Email
        {
            get { return _email; }
            internal set { _email = value; }
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
            internal set { _confirmationCode = value; }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            internal set { _isComplete = value; }
        }

        public bool CanResend
        {
            get { return _canResend; }
            internal set { _canResend = value; }
        }

        public TimeSpan ResendTimeLeft
        {
            get { return _resendTimeLeft; }
            internal set { _resendTimeLeft = value; }
        }

        #endregion
    }
}
