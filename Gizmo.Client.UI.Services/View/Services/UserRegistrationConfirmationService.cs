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
    [Route(ClientRoutes.RegistrationConfirmationRoute)]
    public sealed class UserRegistrationConfirmationService : ValidatingViewStateServiceBase<UserRegistrationConfirmationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationService(UserRegistrationConfirmationViewState viewState,
            ILogger<UserRegistrationConfirmationService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserRegistrationViewState userRegistrationViewState,
            UserRegistrationConfirmationMethodService userRegistrationConfirmationMethodService,
            UserRegistrationConfirmationMethodViewState userRegistrationConfirmationMethodViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
            _userRegistrationViewState = userRegistrationViewState;
            _userRegistrationConfirmationMethodService = userRegistrationConfirmationMethodService;
            _userRegistrationConfirmationMethodViewState = userRegistrationConfirmationMethodViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly UserRegistrationConfirmationMethodService _userRegistrationConfirmationMethodService;
        private readonly UserRegistrationConfirmationMethodViewState _userRegistrationConfirmationMethodViewState;
        #endregion

        #region FUNCTIONS

        public async Task SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            await ValidatePropertyAsync((x) => x.ConfirmationCode);
            DebounceViewStateChanged();

        }

        public void Clear()
        {
            ViewState.ConfirmationCode = string.Empty;
        }

        public Task SMSFallbackAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationMethodRoute);

            return _userRegistrationConfirmationMethodService.SubmitAsync(true);
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
                if (!await _gizmoClient.TokenIsValidAsync(TokenType.CreateAccount, _userRegistrationConfirmationMethodViewState.Token, ViewState.ConfirmationCode))
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("CONFIRMATION_CODE_IS_INVALID");
                    //TODO: AAA CLEAR ERROR WITH TIMER OR SOMETHING?
                    return;
                }

                NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Check create account token validity error.");

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
            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email)
            {
                ViewState.ConfirmationCodeMessage = _localizationService.GetString("CONFIRMATION_EMAIL_MESSAGE", _userRegistrationConfirmationMethodViewState.Destination);
            }
            else if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone)
            {
                if (_userRegistrationConfirmationMethodViewState.DeliveryMethod == ConfirmationCodeDeliveryMethod.FlashCall)
                {
                    ViewState.ConfirmationCodeMessage = _localizationService.GetString("CONFIRMATION_FLASH_CALL_MESSAGE", _userRegistrationConfirmationMethodViewState.Destination, _userRegistrationConfirmationMethodViewState.CodeLength);
                }
                else
                {
                    ViewState.ConfirmationCodeMessage = _localizationService.GetString("CONFIRMATION_SMS_MESSAGE", _userRegistrationConfirmationMethodViewState.Destination);
                }
            }

            return Task.CompletedTask;
        }

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != _userRegistrationConfirmationMethodViewState.CodeLength)
                {
                    validationMessageStore.Add(() => ViewState.ConfirmationCode, _localizationService.GetString("GIZ_CONFIRMATION_CODE_LENGTH_ERROR", _userRegistrationConfirmationMethodViewState.CodeLength));
                }
            }
        }

        #endregion
    }
}
