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
        private static readonly TimeSpan QrExpiryDelay = TimeSpan.FromMinutes(4);
        private static readonly TimeSpan TokenPollInterval = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan TransientFailureRetryDelay = TimeSpan.FromSeconds(1);
        private const int MaxTransientFailureRetries = 1;

        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _session;
        private readonly ILocalizationService _localizationService;
        private readonly IQrCodeService _qrCodeService;
        private readonly CountdownTimer _timer = new();
        private CancellationTokenSource? _asyncActionCts;

        public PasswordRecoveryConfirmationViewService(
            PasswordRecoveryConfirmationViewState viewState,
            ILogger<PasswordRecoveryConfirmationViewService> logger,
            IServiceProvider serviceProvider,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService session,
            ILocalizationService localizationService,
            IQrCodeService qrCodeService) : base(viewState, logger, serviceProvider)
        {
            _passwordRecoveryService = passwordRecoveryService;
            _session = session;
            _localizationService = localizationService;
            _qrCodeService = qrCodeService;
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
                        _session.SetTokenConfirmed(true);
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

            if (_session.Action != PasswordRecoveryAction.Code)
                return;

            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            try
            {
                var provider = _session.ActiveProvider;
                var identifierKind = _session.IdentifierKind;
                if (provider is null || identifierKind is null)
                {
                    ViewState.HasError = true;
                    ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                    return;
                }

                var result = await _passwordRecoveryService.StartAsync(new PasswordRecoveryStartRequest
                {
                    MethodId = provider.MethodId,
                    IdentifierKind = identifierKind.Value,
                    Value = _session.MatchValue
                });

                if (result is PasswordRecoveryStartResult.CodeInputRequired r)
                {
                    _session.SetStartResult(
                        r.Token,
                        r.Destination ?? _session.Destination,
                        r.CodeLength,
                        r.ExpiresInSeconds);
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
            ViewState.Action = PasswordRecoveryAction.None;
            ViewState.RedirectUrl = null;
            ViewState.QrCode = null;
            ViewState.CallPhoneNumber = null;
            ViewState.IsQrExpired = false;
            ViewState.SecondsLeft = 0;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            DebounceViewStateChanged();
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelTimer();
            CancelAsyncActionPolling();

            if (string.IsNullOrEmpty(_session.Token))
            {
                NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);
                return Task.CompletedTask;
            }

            ViewState.ConfirmationCode = string.Empty;
            ViewState.Action = _session.Action;
            ViewState.RedirectUrl = _session.RedirectUrl;
            ViewState.QrCode = null;
            ViewState.CallPhoneNumber = _session.CallPhoneNumber;
            ViewState.IsQrExpired = false;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();

            switch (_session.Action)
            {
                case PasswordRecoveryAction.Code:
                    var channel = _session.ActiveProvider?.Channel ?? PasswordRecoveryChannel.Email;
                    ViewState.ConfirmationCodeMessage = channel == PasswordRecoveryChannel.Email
                        ? _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_EMAIL_MESSAGE), _session.Destination)
                        : _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_PASSWORD_RECOVERY_PLEASE_ENTER_RECOVERY_CODE), _session.Destination);
                    ViewState.RaiseChanged();
                    _ = StartTimerAsync();
                    break;

                case PasswordRecoveryAction.Redirect:
                    if (string.IsNullOrEmpty(_session.RedirectUrl))
                    {
                        Logger.LogWarning("Password recovery redirect result carried no redirect url.");
                        NavigateBackWithFailure();
                        break;
                    }

                    ViewState.ConfirmationCodeMessage = string.Empty;
                    ViewState.QrCode = _qrCodeService.GenerateFromUrl(_session.RedirectUrl);
                    ViewState.RaiseChanged();
                    StartAsyncActionPolling();
                    StartQrExpiryTimer();
                    break;

                case PasswordRecoveryAction.Call:
                    ViewState.ConfirmationCodeMessage = string.Empty;
                    ViewState.RaiseChanged();
                    StartAsyncActionPolling();
                    break;

                default:
                    NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);
                    break;
            }

            return Task.CompletedTask;
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelTimer();
            CancelAsyncActionPolling();
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
            const int seconds = 90;
            return _timer.StartAsync(seconds, secs =>
            {
                ViewState.SecondsLeft = secs;
                ViewState.RaiseChanged();
                return Task.CompletedTask;
            }, Logger);
        }

        private void CancelTimer() => _timer.Cancel();

        private void StartAsyncActionPolling()
        {
            CancelAsyncActionPolling();
            _asyncActionCts = new CancellationTokenSource();
            _ = PollTokenConfirmationAsync(_asyncActionCts.Token);
        }

        private async Task PollTokenConfirmationAsync(CancellationToken cancellationToken)
        {
            var transientFailureCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _passwordRecoveryService.IsTokenConfirmedAsync(_session.Token, cancellationToken);
                    transientFailureCount = 0;

                    if (result.IsConfirmed)
                    {
                        _session.SetTokenConfirmed(true);
                        CancelAsyncActionPolling();
                        NavigationService.NavigateTo(ClientRoutes.PasswordRecoverySetNewPasswordRoute);
                        return;
                    }

                    await Task.Delay(TokenPollInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (transientFailureCount < MaxTransientFailureRetries)
                    {
                        transientFailureCount++;
                        Logger.LogWarning(ex, "Password recovery token polling failed. Retrying {Attempt}/{MaxAttempts}.", transientFailureCount, MaxTransientFailureRetries);

                        try
                        {
                            await Task.Delay(TransientFailureRetryDelay, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }

                        continue;
                    }

                    Logger.LogError(ex, "Password recovery token polling failed.");
                    NavigateBackWithFailure();
                    return;
                }
            }
        }

        private void StartQrExpiryTimer()
        {
            var token = _asyncActionCts?.Token;
            if (token is null)
                return;

            _ = ExpireQrAfterDelayAsync(token.Value);
        }

        private async Task ExpireQrAfterDelayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(QrExpiryDelay, cancellationToken);
                ViewState.IsQrExpired = true;
                ViewState.RaiseChanged();
                CancelAsyncActionPolling();
            }
            catch (OperationCanceledException)
            {
                // Navigation or successful confirmation cancelled the timer.
            }
        }

        private void NavigateBackWithFailure()
        {
            var channelGuid = _session.ActiveProvider?.ChannelGuid;
            _session.SetFailedProviderChannelGuid(channelGuid);
            _session.SetShowAllProviders(true);
            CancelAsyncActionPolling();
            NavigationService.NavigateTo(ClientRoutes.PasswordRecoveryRoute);
        }

        private void CancelAsyncActionPolling()
        {
            if (_asyncActionCts is null)
                return;

            _asyncActionCts.Cancel();
            _asyncActionCts.Dispose();
            _asyncActionCts = null;
        }
    }
}
