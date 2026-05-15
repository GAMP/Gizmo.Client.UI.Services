using Gizmo.Client.UI.Services;
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
    [Route(ClientRoutes.RegistrationConfirmationRoute)]
    public sealed class UserRegistrationConfirmationViewService : ValidatingViewStateServiceBase<UserRegistrationConfirmationViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationViewService(UserRegistrationConfirmationViewState viewState,
            ILogger<UserRegistrationConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IUserRegistrationService registrationService,
            UserRegistrationViewState userRegistrationViewState,
            UserRegistrationConfirmationMethodViewService userRegistrationConfirmationMethodService,
            UserRegistrationConfirmationMethodViewState userRegistrationConfirmationMethodViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _registrationService = registrationService;
            _userRegistrationViewState = userRegistrationViewState;
            _userRegistrationConfirmationMethodService = userRegistrationConfirmationMethodService;
            _userRegistrationConfirmationMethodViewState = userRegistrationConfirmationMethodViewState;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IUserRegistrationService _registrationService;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly UserRegistrationConfirmationMethodViewService _userRegistrationConfirmationMethodService;
        private readonly UserRegistrationConfirmationMethodViewState _userRegistrationConfirmationMethodViewState;
        #endregion

        #region FUNCTIONS

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ValidateProperty(() => ViewState.ConfirmationCode);
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

        public Task SMSFallbackAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationMethodRoute + $"?Fallback=true");

            return _userRegistrationConfirmationMethodService.SubmitAsync(true);
        }

        public async Task Confirm()
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

            try
            {
                var result = await _registrationService.ConfirmTokenAsync(
                    _userRegistrationConfirmationMethodViewState.Token,
                    ViewState.ConfirmationCode);

                if (result == RegistrationConfirmCode.Success)
                {
                    NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                }
                else
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_CONFIRMATION_CODE_IS_INVALID");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Confirm token error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
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
                ViewState.ConfirmationCodeMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_EMAIL_MESSAGE", _userRegistrationConfirmationMethodViewState.Destination);
            }
            else if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone)
            {
                // TODO: FlashCall removed in new DeliveryMethod; re-evaluate when provider-specific fallback is implemented
                ViewState.ConfirmationCodeMessage = _localizationService.GetString("GIZ_USER_CONFIRMATION_SMS_MESSAGE", _userRegistrationConfirmationMethodViewState.Destination);
            }

            return Task.CompletedTask;
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != _userRegistrationConfirmationMethodViewState.CodeLength)
                {
                    AddError(() => ViewState.ConfirmationCode, _localizationService.GetString("GIZ_CONFIRMATION_CODE_LENGTH_ERROR", _userRegistrationConfirmationMethodViewState.CodeLength));
                }
            }
        }

        #endregion

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }
    }
}
