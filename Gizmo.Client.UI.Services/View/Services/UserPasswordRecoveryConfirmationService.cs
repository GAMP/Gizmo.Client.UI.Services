using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserPasswordRecoveryConfirmationService : ValidatingViewStateServiceBase<UserPasswordRecoveryConfirmationViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryConfirmationService(UserPasswordRecoveryConfirmationViewState viewState,
            UserPasswordRecoveryMethodViewState methodState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryMethodState = methodState;
        }
        #endregion

        #region FIELDS
        private readonly UserPasswordRecoveryMethodViewState _passwordRecoveryMethodState;
        #endregion

        #region FUNCTIONS

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);
            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.ConfirmationCode) && ViewState.ConfirmationCode.Length != 6)
            {
                validationMessageStore.Add(() => ViewState.ConfirmationCode, "Confirmation code should have 6 digits!");
            }
        }

        #endregion

    }
}
