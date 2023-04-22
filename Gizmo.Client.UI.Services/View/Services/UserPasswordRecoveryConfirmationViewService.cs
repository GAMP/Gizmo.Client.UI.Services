using Gizmo.Client.UI.View.States;
using Gizmo.UI;
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
    public sealed class UserPasswordRecoveryConfirmationViewService : ValidatingViewStateServiceBase<UserPasswordRecoveryConfirmationViewState>
    {
        #region CONTRUCTOR
        public UserPasswordRecoveryConfirmationViewService(UserPasswordRecoveryConfirmationViewState viewState,
            ILogger<UserPasswordRecoveryViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserPasswordRecoveryViewService userPasswordRecoveryService,
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
        private readonly UserPasswordRecoveryViewService _userPasswordRecoveryService;
        private readonly UserPasswordRecoveryViewState _userPasswordRecoveryViewState;
        #endregion

        #region FUNCTIONS

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ValidateProperty(() => ViewState.ConfirmationCode);
        }

        public Task SMSFallbackAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);

            return _userPasswordRecoveryService.SubmitAsync(true);
        }

        public void Confirm()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                return;
            }

            NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);

            ViewState.IsLoading = false;
            ViewState.RaiseChanged();
        }

        public void Clear()
        {
            ViewState.ConfirmationCode = string.Empty;
            ViewState.ConfirmationCodeMessage = string.Empty;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
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

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != _userPasswordRecoveryViewState.CodeLength)
                {
                    AddError(() => ViewState.ConfirmationCode, _localizationService.GetString("GIZ_CONFIRMATION_CODE_LENGTH_ERROR", _userPasswordRecoveryViewState.CodeLength));
                }
            }
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode))
            {
                if (!string.IsNullOrEmpty(ViewState.ConfirmationCode))
                {
                    try
                    {
                        if (!await _gizmoClient.TokenIsValidAsync(TokenType.ResetPassword, _userPasswordRecoveryViewState.Token, ViewState.ConfirmationCode))
                        {
                            return new string[] { _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID") };
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Check password recovery token validity error.");
                        return new string[] { _localizationService.GetString("VE_ERROR_CANNOT_VALIDATE_REGISTRATION_TOKEN") };
                    }
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        #endregion
    }
}
