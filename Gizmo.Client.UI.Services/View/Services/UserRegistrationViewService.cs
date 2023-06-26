using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationViewService : ValidatingViewStateServiceBase<UserRegistrationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationViewService(UserRegistrationViewState viewState,
            ILogger<UserRegistrationViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        public void SetConfirmationMethod(RegistrationVerificationMethod value)
        {
            ViewState.ConfirmationMethod = value;
            DebounceViewStateChanged();
        }

        public void SetUserGroupDefaultRequiredInfo(UserModelRequiredInfo value)
        {
            ViewState.DefaultUserGroupRequiredInfo = value;
            DebounceViewStateChanged();
        }
    }
}
