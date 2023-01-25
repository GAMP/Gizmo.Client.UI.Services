using Gizmo.UI;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationAdditionalFieldsViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _homePhone = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets home phone.
        /// </summary>
        [ValidatingProperty()]
        public string HomePhone
        {
            get { return _homePhone; }
            internal set { SetProperty(ref _homePhone, value); }
        }

        public UserModelRequiredInfo DefaultUserGroupRequiredInfo { get; internal set; }

        #endregion
    }
}
