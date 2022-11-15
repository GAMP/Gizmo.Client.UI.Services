using Gizmo.UI;
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
        #endregion

        #region PROPERTIES

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        [ValidatingProperty()]
        [Required()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            set { SetProperty(ref _mobilePhone, value); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }

        [ValidatingProperty()]
        [Required()]
        public string ConfirmationCode
        {
            get { return _confirmationCode; }
            set { SetProperty(ref _confirmationCode, value); }
        }

        public bool IsComplete
        {
            get { return _isComplete; }
            set { SetProperty(ref _isComplete, value); }
        }

        #endregion
    }
}
