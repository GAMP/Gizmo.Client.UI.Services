using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public UserRegistrationMethod ConfirmationMethod { get; internal set; }

        public UserModelRequiredInfo DefaultUserGroupRequiredInfo { get; internal set; } = new UserModelRequiredInfo()
        {
            FirstName = true,
            LastName = true,
            BirthDate = true,
            Address = true,
            City = true,
            PostCode = true,
            State = true,
            Country = true,
            Email = true,
            Phone = true,
            Mobile = true,
            Sex = true,
            Password = true
        };

        public string Username { get; internal set; } = null!;

        public string Password { get; internal set; } = null!;

        public string FirstName { get; internal set; } = null!;

        public string LastName { get; internal set; } = null!;

        public DateTime? BirthDate { get; internal set; }

        public Sex Sex { get; internal set; }

        public string Email { get; internal set; } = null!;

        public string Country { get; internal set; } = null!;

        public string MobilePhone { get; internal set; } = null!;

        public string Address { get; internal set; } = null!;

        public string PostCode { get; internal set; } = null!;

        public IEnumerable<UserAgreementModelState> UserAgreementStates { get; internal set; } = Enumerable.Empty<UserAgreementModelState>();

        public string Token { get; internal set; } = null!;

        #endregion
    }
}
