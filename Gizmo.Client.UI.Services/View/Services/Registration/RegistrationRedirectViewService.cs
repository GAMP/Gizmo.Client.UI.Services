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
    public sealed class RegistrationRedirectViewService : ViewStateServiceBase<RegistrationRedirectViewState>
    {
        private static readonly TimeSpan QrExpiryDelay = TimeSpan.FromMinutes(4);

        // TODO: Временное решение — обычный polling каждые 20 сек, 12 попыток (4 мин, 3 запроса в минуту).
        // Заменить на long polling после переделки IsTokenConfirmedAsync на сервере.
        private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(20);
        private const int PollingMaxAttempts = 12;

        #region CONSTRUCTOR
        public RegistrationRedirectViewService(
            RegistrationRedirectViewState viewState,
            ILogger<RegistrationRedirectViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            UserRegistrationViewState userRegistrationViewState,
            RegistrationProvidersViewService registrationProvidersViewService,
            IQrCodeService qrCodeService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _userRegistrationViewState = userRegistrationViewState;
            _registrationProvidersViewService = registrationProvidersViewService;
            _qrCodeService = qrCodeService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly RegistrationProvidersViewService _registrationProvidersViewService;
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
            var provider = _userRegistrationViewState.SelectedProvider;
            var integrationPublicId = provider?.PublicId ?? Guid.Empty;
            var channelGuid = provider?.ChannelGuid ?? Guid.Empty;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    DeliveryMethod = RegistrationDeliveryMethod.Redirect,
                    IntegrationPublicId = integrationPublicId
                }, cancellationToken);

                switch (result.Result)
                {
                    case RegistrationStartCode.Success when result.RedirectUrl is not null:
                        _registrationSession.SetStartResult(
                            result.Token ?? string.Empty,
                            result.Destination ?? string.Empty,
                            result.CodeLength,
                            result.ExpiresInSeconds,
                            RegistrationFlow.None);
                        ViewState.RedirectUrl = result.RedirectUrl;
                        ViewState.QrCode = _qrCodeService.GenerateFromUrl(result.RedirectUrl);
                        StartQrExpiryTimer();
                        _ = StartTokenPollingAsync(_qrExpiryCts!.Token);
                        break;

                    default:
                        Logger.LogWarning("Redirect registration start failed: {Result}", result.Result);
                        _registrationProvidersViewService.SetProviderError(channelGuid);
                        NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Redirect registration start error.");
                _registrationProvidersViewService.SetProviderError(channelGuid);
                NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
            }
        }

        //TODO временное решение, заменить после правки на сервере
        private async Task StartTokenPollingAsync(CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < PollingMaxAttempts; attempt++)
            {
                try
                {
                    await Task.Delay(PollingInterval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                try
                {
                    var result = await _registrationService.IsTokenConfirmedAsync(
                        _registrationSession.Token, cancellationToken);

                    if (result.UserAlreadyExists)
                    {
                        CancelQrExpiry();
                        NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                        return;
                    }

                    if (result.IsConfirmed)
                    {
                        CancelQrExpiry();
                        NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                        return;
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Ошибка при проверке статуса токена (попытка {Attempt}).", attempt + 1);
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
