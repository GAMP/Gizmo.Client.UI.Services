using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
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
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ViewState.RaiseChanged();
        }

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

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != 6)
                {
                    validationMessageStore.Add(() => ViewState.ConfirmationCode, "Confirmation code should have 6 digits!"); //TODO: A TRANSLATE
                }
                else
                {
                    TokenType tokenType = TokenType.CreateAccount;
                    string token = string.Empty; //TODO: A Get token

                    if (!await _gizmoClient.TokenIsValidAsync(tokenType, token, ViewState.ConfirmationCode))
                    {
                        validationMessageStore.Add(() => ViewState.ConfirmationCode, _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID"));
                    }
                }
            }
        }

        #endregion
    }
}
