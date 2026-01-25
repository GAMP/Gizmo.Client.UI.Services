using Gizmo.UI.View.States;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Gizmo.Client.UI.View.States
{
    [Register]
    public sealed class UserRegistrationViewState : ValidatingViewStateBase
    {
        #region PROPERTIES

        public Server.RegistrationVerificationMethod ConfirmationMethod { get; internal set; } = Server.RegistrationVerificationMethod.None;

        public UserModelRequiredInfo DefaultUserGroupRequiredInfo { get; internal set; } = new UserModelRequiredInfo();

        #endregion
    }
}
