using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public sealed class UserRegistrationConfirmationViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _confirmationCode = String.Empty;
        #endregion

        #region PROPERTIES

        [ValidatingProperty()]
        [Required()]
        public string ConfirmationCode
        {
            get { return _confirmationCode; }
            set { SetProperty(ref _confirmationCode, value); }
        }

        #endregion
    }
}
