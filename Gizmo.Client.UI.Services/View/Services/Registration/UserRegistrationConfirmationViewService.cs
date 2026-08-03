using Gizmo.Client;
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
        public UserRegistrationConfirmationViewService(
            UserRegistrationConfirmationViewState viewState,
            ILogger<UserRegistrationConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _registrationService = registrationService;
            _registrationSession = registrationSession;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly CountdownTimer _timer = new();
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

        public async Task Confirm()
        {
            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
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
                    _registrationSession.Token,
                    ViewState.ConfirmationCode);

                switch (result)
                {
                    case RegistrationConfirmCode.Success:
                        NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                        break;

                    case RegistrationConfirmCode.InvalidToken:
                    case RegistrationConfirmCode.InvalidConfirmationCode:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_CONFIRMATION_CODE_IS_INVALID));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Confirm token error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        public async ValueTask RestartTimerAsync()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var request = new RegistrationStartRequest
                {
                    MethodId = _registrationSession.SelectedProvider?.MethodId ?? 0,
                    Kind = _registrationSession.Flow == RegistrationFlow.Email
                        ? RegistrationStartKind.Email
                        : RegistrationStartKind.MobilePhone,
                    Value = _registrationSession.ActualContact,
                };

                var result = await _registrationService.StartAsync(request);

                if (result is RegistrationStartResult.CodeInputRequired r)
                {
                    _registrationSession.SetStartResult(
                        r.Token,
                        r.Destination ?? _registrationSession.Destination,
                        r.CodeLength,
                        r.ExpiresInSeconds,
                        _registrationSession.Flow);
                    _ = StartTimerAsync();
                }
                else
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                    ViewState.RaiseChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Resend confirmation code error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                ViewState.RaiseChanged();
            }
        }

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_registrationSession.Token))
            {
                NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                return Task.CompletedTask;
            }

            ViewState.ConfirmationCodeMessage = _registrationSession.Flow == RegistrationFlow.Email
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_EMAIL_MESSAGE), _registrationSession.Destination)
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_SMS_MESSAGE), _registrationSession.Destination);

            ViewState.ConfirmationCode = string.Empty;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();

            _ = StartTimerAsync();

            return Task.CompletedTask;
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelTimer();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        private Task StartTimerAsync()
        {
            const int seconds = 90;

            return _timer.StartAsync(seconds, secs =>
            {
                ViewState.SecondsLeft = secs;
                ViewState.RaiseChanged();
                return Task.CompletedTask;
            }, Logger);
        }

        private void CancelTimer() => _timer.Cancel();

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode))
            {
                if (ViewState.ConfirmationCode.Length != _registrationSession.CodeLength)
                {
                    AddError(() => ViewState.ConfirmationCode, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_CONFIRMATION_CODE_LENGTH_ERROR), _registrationSession.CodeLength));
                }
            }
        }

        #endregion
    }
}
