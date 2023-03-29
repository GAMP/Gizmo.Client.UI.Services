using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationViewStateService : ValidatingViewStateServiceBase<UserRegistrationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationViewStateService(UserRegistrationViewState viewState,
            ILogger<UserRegistrationViewStateService> logger,
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
