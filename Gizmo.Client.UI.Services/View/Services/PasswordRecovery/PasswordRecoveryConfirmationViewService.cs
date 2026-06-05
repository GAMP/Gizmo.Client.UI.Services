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
    [Register]
    [Route(ClientRoutes.PasswordRecoveryConfirmationRoute)]
    public sealed class PasswordRecoveryConfirmationViewService : ValidatingViewStateServiceBase<PasswordRecoveryConfirmationViewState>
    {
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;
        private readonly CountdownTimer _timer = new();

        public PasswordRecoveryConfirmationViewService(
            PasswordRecoveryConfirmationViewState viewState,
            ILogger<PasswordRecoveryConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
        }

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ValidateProperty(() => ViewState.ConfirmationCode);
        }

        public async Task Confirm()
        {
            if (ViewState.IsLoading)
                return;

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
                var result = await _passwordRecoveryService.ConfirmCodeAsync(_session.Token, ViewState.ConfirmationCode);

                switch (result)
                {
                    case PasswordRecoveryConfirmCode.Success:
                        _session.SetCodeConfirmed(true);
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);
                        break;

                    case PasswordRecoveryConfirmCode.InvalidToken:
                    case PasswordRecoveryConfirmCode.ExpiredToken:
                    case PasswordRecoveryConfirmCode.UsedToken:
                    case PasswordRecoveryConfirmCode.InvalidConfirmationCode:
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
                Logger.LogError(ex, "Confirm password recovery code error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public async ValueTask RestartTimerAsync()
        {
            if (ViewState.IsLoading)
                return;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var provider = _session.ActiveProvider;
                if (provider is null)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                    return;
                }

                var result = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    IntegrationPublicId = provider.PublicId,
                    Channel = provider.Channel,
                    MatchValue = _session.MatchValue
                });

                if (result.Result == PasswordRecoveryStartCode.Success)
                {
                    _session.SetStartResult(
                        result.Token ?? string.Empty,
                        result.Destination ?? _session.Destination,
                        result.CodeLength,
                        result.ExpiresInSeconds);
                    _ = StartTimerAsync();
                }
                else
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Resend recovery code error.");
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

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_session.Token))
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);
                return Task.CompletedTask;
            }

            var channel = _session.ActiveProvider?.Channel ?? PasswordRecoveryChannel.Email;
            ViewState.ConfirmationCodeMessage = channel == PasswordRecoveryChannel.Email
                ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_EMAIL_MESSAGE), _session.Destination)
                : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PLEASE_ENTER_RECOVERY_CODE), _session.Destination);

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

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode) &&
                ViewState.ConfirmationCode.Length != _session.CodeLength)
            {
                AddError(() => ViewState.ConfirmationCode, _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_CONFIRMATION_CODE_LENGTH_ERROR), _session.CodeLength));
            }
        }

        private Task StartTimerAsync()
        {
            var seconds = _session.ExpiresInSeconds > 0 ? _session.ExpiresInSeconds : 60;
            return _timer.StartAsync(seconds, secs =>
            {
                ViewState.SecondsLeft = secs;
                ViewState.RaiseChanged();
                return Task.CompletedTask;
            }, Logger);
        }

        private void CancelTimer() => _timer.Cancel();
    }
}
