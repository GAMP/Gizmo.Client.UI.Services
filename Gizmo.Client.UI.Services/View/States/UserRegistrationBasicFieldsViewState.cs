using Gizmo.UI;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationBasicFieldsViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or sets username.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string Username { get; internal set; } = null!;

        /// <summary>
        /// Gets or sets new password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string NewPassword { get; internal set; } = null!;

        /// <summary>
        /// Gets or sets repeat password.
        /// </summary>
        [ValidatingProperty()]
        [Required()]
        public string RepeatPassword { get; internal set; } = null!;

        [ValidatingProperty()]
        public string FirstName { get; internal set; } = null!;

        [ValidatingProperty()]
        public string LastName { get; internal set; } = null!;

        [ValidatingProperty()]
        public DateTime? BirthDate { get; internal set; }

        [ValidatingProperty()]
        public Sex Sex { get; internal set; }

        [ValidatingProperty()]
        public string? Email { get; internal set; }

        #endregion
    }
}
