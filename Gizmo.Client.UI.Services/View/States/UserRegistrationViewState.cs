using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public UserRegistrationMethod ConfirmationMethod { get; internal set; } = UserRegistrationMethod.MobilePhone; //TODO: A DEMO

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

        public string? Token { get; internal set; }

        #endregion
    }
}
