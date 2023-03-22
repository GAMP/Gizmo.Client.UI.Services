using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _mobilePhone = string.Empty;
        private string _email = string.Empty;
        private bool _isLoading;
        #endregion

        #region PROPERTIES

        public UserRecoveryMethod SelectedRecoveryMethod { get; internal set; }

        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        [ValidatingProperty()]
        public string Email
        {
            get { return _email; }
            internal set { SetProperty(ref _email, value); }
        }

        public string? Token { get; internal set; }

        public bool IsLoading
        {
            get { return _isLoading; }
            internal set { _isLoading = value; }
        }

        #endregion
    }
}
