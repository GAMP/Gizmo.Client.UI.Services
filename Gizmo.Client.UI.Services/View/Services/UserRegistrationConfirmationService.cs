using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationConfirmationService : ValidatingViewStateServiceBase<UserRegistrationConfirmationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationService(UserRegistrationConfirmationViewState viewState,
            ILogger<UserRegistrationConfirmationService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
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
