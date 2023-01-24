﻿using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserPasswordRecoveryConfirmationViewState : ValidatingViewStateBase
    {
        #region FIELDS
        private string _confirmationCode = string.Empty;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets confirmation code.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string ConfirmationCode
        {
            get { return _confirmationCode; }
            internal set { SetProperty(ref _confirmationCode, value); }
        }

        #endregion
    }
}
