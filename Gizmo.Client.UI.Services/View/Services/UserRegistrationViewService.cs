using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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

        [Obsolete("Use SelectProvider via RegistrationProvidersViewService instead.")]
        public void SetConfirmationMethod(RegistrationVerificationMethod value)
        {
            ViewState.ConfirmationMethod = value;
            DebounceViewStateChanged();
        }

        public void SelectProvider(RegistrationProvider provider)
        {
            ViewState.SelectedProvider = provider;
            DebounceViewStateChanged();
        }

        public void ClearProvider()
        {
            ViewState.SelectedProvider = null;
            DebounceViewStateChanged();
        }

        public void SetUserGroupDefaultRequiredInfo(RegistrationRequiredInfo? value)
        {
            ViewState.DefaultUserGroupRequiredInfo = value;
            DebounceViewStateChanged();
        }
    }
}
