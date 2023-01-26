using Gizmo.UI;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationAdditionalFieldsViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _address = string.Empty;
        private string _postCode = string.Empty;
        private string _mobilePhone = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        [ValidatingProperty()]
        public string Address
        {
            get { return _address; }
            internal set { SetProperty(ref _address, value); }
        }

        /// <summary>
        /// Gets or sets post code.
        /// </summary>
        [ValidatingProperty()]
        public string PostCode
        {
            get { return _postCode; }
            internal set { SetProperty(ref _postCode, value); }
        }

        /// <summary>
        /// Gets or sets mobile phone.
        /// </summary>
        [ValidatingProperty()]
        public string MobilePhone
        {
            get { return _mobilePhone; }
            internal set { SetProperty(ref _mobilePhone, value); }
        }

        public UserModelRequiredInfo DefaultUserGroupRequiredInfo { get; internal set; }

        #endregion
    }
}
