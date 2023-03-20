using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public RegistrationVerificationMethod ConfirmationMethod { get; internal set; } = RegistrationVerificationMethod.MobilePhone; //TODO: A DEMO

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

        #endregion
    }
}
