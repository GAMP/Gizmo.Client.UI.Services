using Gizmo.UI;
using Gizmo.UI.View.States;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserSettingsViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string? Username { get; internal set; }

        [ValidatingProperty()]
        public string? FirstName { get; internal set; }

        [ValidatingProperty()]
        public string? LastName { get; internal set; }

        [ValidatingProperty()]
        public DateTime? BirthDate { get; internal set; }

        [ValidatingProperty()]
        public Sex Sex { get; internal set; }

        [ValidatingProperty()]
        public string? Country { get; internal set; }

        [ValidatingProperty()]
        public string? Address { get; internal set; }

        [ValidatingProperty()]
        public string? Email { get; internal set; }

        [ValidatingProperty()]
        public string? Phone { get; internal set; }

        [ValidatingProperty()]
        public string? MobilePhone { get; internal set; }

        #endregion
    }
}
