using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
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
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserPasswordRecoveryViewState userPasswordRecoveryViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _userPasswordRecoveryViewState = userPasswordRecoveryViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserPasswordRecoveryViewState _userPasswordRecoveryViewState;
        #endregion

        #region FUNCTIONS

        public async Task SetConfirmationCode(string value)
        {
            using (ViewStateChangeDebounced())
            {
                ViewState.ConfirmationCode = value;
                await ValidatePropertyAsync((x)=>x.ConfirmationCode);
            }
        }

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

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            await base.OnCustomValidationAsync(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != 6)
                {
                    validationMessageStore.Add(() => ViewState.ConfirmationCode, "Confirmation code should have 6 digits!"); //TODO: A TRANSLATE
                }
                else
                {
                    if (!await _gizmoClient.TokenIsValidAsync(TokenType.ResetPassword, _userPasswordRecoveryViewState.Token, ViewState.ConfirmationCode))
                    {
                        validationMessageStore.Add(() => ViewState.ConfirmationCode, _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID"));
                    }
                }
            }
        }

        #endregion
    }
}
