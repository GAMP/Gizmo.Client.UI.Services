using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public sealed class UserPasswordRecoveryConfirmationService : ValidatingViewStateServiceBase<UserPasswordRecoveryConfirmationViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryConfirmationService(UserPasswordRecoveryConfirmationViewState viewState,
            ILogger<UserPasswordRecoveryService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserPasswordRecoveryService userPasswordRecoveryService,
            UserPasswordRecoveryViewState userPasswordRecoveryViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _userPasswordRecoveryService = userPasswordRecoveryService;
            _userPasswordRecoveryViewState = userPasswordRecoveryViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserPasswordRecoveryService _userPasswordRecoveryService;
        private readonly UserPasswordRecoveryViewState _userPasswordRecoveryViewState;
        #endregion

        #region FUNCTIONS

        public async Task SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            await ValidatePropertyAsync((x) => x.ConfirmationCode);
            DebounceViewStateChanged();
        }

        public Task SMSFallbackAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);

            return _userPasswordRecoveryService.SubmitAsync(true);
        }

        public async Task ConfirmAsync()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                return;
            }

            try
            {
                if (!await _gizmoClient.TokenIsValidAsync(TokenType.ResetPassword, _userPasswordRecoveryViewState.Token, ViewState.ConfirmationCode))
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID");
                    //TODO: AAA CLEAR ERROR WITH TIMER OR SOMETHING?
                    return;
                }

                NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Check password recovery token validity error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = ex.ToString();
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (_userPasswordRecoveryViewState.SelectedRecoveryMethod == UserRecoveryMethod.Email)
            {
                ViewState.ConfirmationCodeMessage = _localizationService.GetString("CONFIRMATION_EMAIL_MESSAGE", _userPasswordRecoveryViewState.Destination);
            }
            else if (_userPasswordRecoveryViewState.SelectedRecoveryMethod == UserRecoveryMethod.Mobile)
            {
                if (_userPasswordRecoveryViewState.DeliveryMethod == ConfirmationCodeDeliveryMethod.FlashCall)
                {
                    ViewState.ConfirmationCodeMessage = _localizationService.GetString("CONFIRMATION_FLASH_CALL_MESSAGE", _userPasswordRecoveryViewState.Destination, _userPasswordRecoveryViewState.CodeLength);
                }
                else
                {
                    ViewState.ConfirmationCodeMessage = _localizationService.GetString("CONFIRMATION_SMS_MESSAGE", _userPasswordRecoveryViewState.Destination);
                }
            }

            return Task.CompletedTask;
        }

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != _userPasswordRecoveryViewState.CodeLength)
                {
                    validationMessageStore.Add(() => ViewState.ConfirmationCode, _localizationService.GetString("GIZ_CONFIRMATION_CODE_LENGTH_ERROR", _userPasswordRecoveryViewState.CodeLength));
                }
            }
        }

        #endregion
    }
}
