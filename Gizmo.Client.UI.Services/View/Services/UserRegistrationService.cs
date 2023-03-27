using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationService : ValidatingViewStateServiceBase<UserRegistrationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationService(UserRegistrationViewState viewState,
            ILogger<UserRegistrationService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger,serviceProvider)
        {
        }
        #endregion

        public void SetConfirmationMethod(RegistrationVerificationMethod value)
        {
            ViewState.ConfirmationMethod = value;
            DebounceViewStateChanged();
        }
    }
}
