using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserSettingsViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _username = string.Empty;
        private string _homePhone = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        /// <summary>
        /// Gets or sets home phone.
        /// </summary>
        [ValidatingProperty()]
        public string HomePhone
        {
            get { return _homePhone; }
            set { SetProperty(ref _homePhone, value); }
        }

        #endregion
    }
}
