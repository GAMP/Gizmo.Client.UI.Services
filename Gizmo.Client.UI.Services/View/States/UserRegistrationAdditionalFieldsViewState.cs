﻿using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationAdditionalFieldsViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        /// <summary>
        /// Gets or sets mobile phone.
        /// </summary>
        [ValidatingProperty()]
        public string? MobilePhone { get; internal set; }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        [ValidatingProperty()]
        public string? Address { get; internal set; }

        /// <summary>
        /// Gets or sets post code.
        /// </summary>
        [ValidatingProperty()]
        public string? PostCode { get; internal set; }

        #endregion
    }
}
