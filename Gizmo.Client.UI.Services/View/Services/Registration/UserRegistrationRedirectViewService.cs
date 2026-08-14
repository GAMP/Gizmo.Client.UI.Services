using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationRedirectRoute)]
    public sealed class UserRegistrationRedirectViewService : ViewStateServiceBase<UserRegistrationRedirectViewState>
    {
        private static readonly TimeSpan QrExpiryDelay = TimeSpan.FromMinutes(4);
        private static readonly TimeSpan TokenPollInterval = TimeSpan.FromSeconds(2);
        private static readonly TimeSpan TransientFailureRetryDelay = TimeSpan.FromSeconds(1);
        private const int MaxTransientFailureRetries = 1;

        #region CONSTRUCTOR
        public UserRegistrationRedirectViewService(
            UserRegistrationRedirectViewState viewState,
            ILogger<UserRegistrationRedirectViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            IQrCodeService qrCodeService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _qrCodeService = qrCodeService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly IQrCodeService _qrCodeService;
        private CancellationTokenSource? _qrExpiryCts;
        #endregion

        #region FUNCTIONS

        public Task NavigateBackAsync()
        {
            CancelQrExpiry();
            _registrationSession.Clear();
            NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        public Task NavigateToProvidersAsync()
        {
            CancelQrExpiry();
            _registrationSession.Clear();
            NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelQrExpiry();
            ViewState.RedirectUrl = null;
            ViewState.QrCode = null;
            ViewState.IsQrExpired = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                await LoadQrAsync(cancellationToken);
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            CancelQrExpiry();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private async Task LoadQrAsync(CancellationToken cancellationToken)
        {
            var provider = _registrationSession.SelectedProvider;
            var methodId = provider?.MethodId ?? 0;
            var channelGuid = provider?.ChannelGuid ?? Guid.Empty;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    MethodId = methodId,
                    Kind = RegistrationStartKind.Redirect
                }, cancellationToken);

                switch (result)
                {
                    case RegistrationStartResult.RedirectRequired r:
                        _registrationSession.SetStartResult(
                            r.Token,
                            string.Empty,
                            0,
                            r.ExpiresInSeconds,
                            RegistrationFlow.Redirect);
                        ViewState.RedirectUrl = r.RedirectUrl;
                        ViewState.QrCode = _qrCodeService.GenerateFromUrl(r.RedirectUrl);
                        StartQrExpiryTimer();
                        _ = StartTokenPollingAsync(_qrExpiryCts!.Token);
                        break;

                    default:
                        Logger.LogWarning("Redirect registration start failed: {Result}", result);
                        _registrationSession.SetFailedProviderChannelGuid(channelGuid);
                        NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Redirect registration start error.");
                _registrationSession.SetFailedProviderChannelGuid(channelGuid);
                NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
            }
        }

        private async Task StartTokenPollingAsync(CancellationToken cancellationToken)
        {
            var transientFailureCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await _registrationService.IsTokenConfirmedAsync(
                        _registrationSession.Token, cancellationToken);

                    transientFailureCount = 0;

                    if (result.UserAlreadyExists)
                    {
                        CancelQrExpiry();
                        NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                        return;
                    }

                    if (result.IsConfirmed)
                    {
                        CancelQrExpiry();

                        if (!string.IsNullOrEmpty(result.Phone))
                        {
                            var confirmedPhone = result.Phone.StartsWith("+")
                                ? result.Phone.Substring(1)
                                : result.Phone;

                            _registrationSession.SetContactDetails(confirmedPhone);
                        }

                        NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
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
                        Logger.LogWarning(ex, "Ошибка при long polling проверке статуса токена. Повторная попытка {Attempt}/{MaxAttempts}.", transientFailureCount, MaxTransientFailureRetries);

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

                    Logger.LogError(ex, "Ошибка при long polling проверке статуса токена.");
                    CancelQrExpiry();
                    NavigationService.NavigateTo(ClientRoutes.RegistrationErrorRoute);
                    return;
                }
            }
        }

        private void StartQrExpiryTimer()
        {
            CancelQrExpiry();
            _qrExpiryCts = new CancellationTokenSource();
            var token = _qrExpiryCts.Token;
            _ = ExpireQrAfterDelayAsync(token);
        }

        private async Task ExpireQrAfterDelayAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(QrExpiryDelay, cancellationToken);
                ViewState.IsQrExpired = true;
                ViewState.RaiseChanged();
                CancelQrExpiry();
            }
            catch (OperationCanceledException)
            {
                // Navigation or refresh cancelled the timer.
            }
        }

        private void CancelQrExpiry()
        {
            if (_qrExpiryCts is null)
                return;

            _qrExpiryCts.Cancel();
            _qrExpiryCts.Dispose();
            _qrExpiryCts = null;
        }

        #endregion
    }
}
