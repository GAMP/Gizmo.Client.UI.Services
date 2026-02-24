using Gizmo.Client.Options;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Messaging;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Responsible of maintaining host reservation view state.
    /// </summary>
    [Register()]
    public sealed class HostReservationViewService : ViewStateServiceBase<HostReservationViewState>
    {
        public HostReservationViewService(HostReservationViewState viewState,
            ILogger<HostReservationViewService> logger,
            IOptionsMonitor<ClientReservationOptions> reservationOptions,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ConfirmReservationNotificationViewService confirmReservationNotificationViewService,
            ConfirmReservationDialogViewService confirmReservationDialogViewService)
            : base(viewState, logger, serviceProvider)
        {
            _reservationOptions = reservationOptions;
            _gizmoClient = gizmoClient;
            _confirmReservationNotificationViewService = confirmReservationNotificationViewService;
            _confirmReservationDialogViewService = confirmReservationDialogViewService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly ConfirmReservationNotificationViewService _confirmReservationNotificationViewService;
        private readonly ConfirmReservationDialogViewService _confirmReservationDialogViewService;

        private readonly IOptionsMonitor<ClientReservationOptions> _reservationOptions;
        private ClientNextReservationModel? _nextReservation;

        private readonly SemaphoreSlim _reservationRefreshLock = new(1);
        private readonly SemaphoreSlim _reservationWarningLock = new(1);
        private Timer? _reservationRefreshTimer = null;
        private const int RESERVATION_REFFRESH_INTERVAL = 1000;

        private CancellationTokenSource? _warningCancellationTokenSource = null;

        private async Task LoadNextHostReservation()
        {
            StopTimer();

            try
            {
                _nextReservation = await _gizmoClient.ClientReservationGetAsync();

                await ReservationRefresh();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load next host reservation.");
            }

            StartTimer();
        }

        private async void ReservationRefreshCallback(object? state)
        {
            await ReservationRefresh();
        }

        private async Task ReservationRefresh()
        {
            if (await _reservationRefreshLock.WaitAsync(TimeSpan.Zero))
            {
                try
                {
                    var currentTime = DateTime.Now;

                    var reservationId = _nextReservation?.NextReservationId;
                    var reservationTime = _nextReservation?.NextReservationTime;
                    var reservationDuration = _nextReservation?.Duration;
                    var reservationBlockTime = _nextReservation?.LoginBlockBeforeTime;
                    var reservationNotificationTime = _reservationOptions.CurrentValue.AlertBeforeTime;
                    var reservationPaymentStatus = _nextReservation?.PaymentStatus;

                    var reservationTimeReached = false;
                    var reservationBlockTimeReached = false;
                    var reservationNotificationTimeReached = false;

                    //check if we have reservation configuration data and that there is a reservation upcoming
                    if (reservationId != null && reservationTime != null && reservationDuration != null)
                    {
                        if (currentTime >= reservationTime.Value)
                        {
                            reservationTimeReached = true;
                        }

                        if (reservationBlockTime.HasValue)
                        {
                            if (currentTime >= reservationTime.Value.AddMinutes(reservationBlockTime.Value * -1))
                            {
                                reservationBlockTimeReached = true;
                            }
                        }
                        else
                        {
                            if (currentTime >= reservationTime.Value)
                            {
                                reservationBlockTimeReached = true;
                            }
                        }

                        if (reservationNotificationTime.HasValue)
                        {
                            if (currentTime >= reservationTime.Value.AddMinutes(reservationNotificationTime.Value * -1))
                            {
                                reservationNotificationTimeReached = true;
                            }
                        }
                    }

                    //Update UI only if there are changes.
                    if (ViewState.ReservationId != reservationId ||
                        ViewState.Time != reservationTime ||
                        ViewState.Duration != reservationDuration ||
                        ViewState.ReservationTimeReached != reservationTimeReached ||
                        ViewState.ReservationBlockTimeReached != reservationBlockTimeReached ||
                        ViewState.ReservationNotificationTimeReached != reservationNotificationTimeReached ||
                        ViewState.ReservationPaymentStatus != reservationPaymentStatus)
                    {
                        if (_warningCancellationTokenSource != null)
                        {
                            _warningCancellationTokenSource.Cancel();
                        }

                        if (ViewState.ReservationId != reservationId)
                        {
                            ViewState.Ignored = false;
                            ViewState.DismissedTime = null;
                        }

                        var previousReservationTimeReached = ViewState.ReservationTimeReached;
                        var previousReservationBlockTimeReached = ViewState.ReservationBlockTimeReached;
                        var previousReservationNotificationTimeReached = ViewState.ReservationNotificationTimeReached;

                        ViewState.ReservationId = reservationId;
                        ViewState.Time = reservationTime;
                        ViewState.Duration = reservationDuration;
                        ViewState.ReservationTimeReached = reservationTimeReached;
                        ViewState.ReservationBlockTimeReached = reservationBlockTimeReached;
                        ViewState.ReservationNotificationTimeReached = reservationNotificationTimeReached;
                        ViewState.ReservationPaymentStatus = reservationPaymentStatus;

                        DebounceViewStateChanged();

                        if (ViewState.ReservationId.HasValue)
                        {
                            if (_gizmoClient.IsUserLoggedIn)
                            {
                                if (!reservationTimeReached && !ViewState.Ignored)
                                {
                                    if (reservationNotificationTime.HasValue)
                                    {
                                        if (!previousReservationNotificationTimeReached && reservationNotificationTimeReached)
                                        {
                                            _ = ShowNotification();
                                        }
                                        else if (!previousReservationBlockTimeReached && reservationBlockTimeReached)
                                        {

                                        }
                                    }
                                    else if (reservationBlockTime.HasValue)
                                    {
                                        if (_gizmoClient.IsUserLoggedIn)
                                        {
                                            //The notification time is not set but block time is set.
                                            if (!previousReservationBlockTimeReached && reservationBlockTimeReached)
                                            {
                                                //Show notification for payment
                                                //If not confirmed will be logged out. //TODO: AAAAA CHECK THAT THE DIALOG WILL BE CLOSED IN THIS CASE.
                                                //TODO: AAAAA ONLY CONFIRMED
                                                _ = ShowNotification();
                                            }
                                        }
                                    }
                                }

                                if (reservationTimeReached && !previousReservationTimeReached)
                                {
                                    //If not confirmed will be logged out. //TODO: AAAAA CHECK THAT THE DIALOG WILL BE CLOSED IN THIS CASE.
                                    _ = ShowDialog();
                                }
                            }
                        }
                        else
                        {
                            StopTimer();
                            return;
                        }
                    }

                    if (_gizmoClient.IsUserLoggedIn)
                    {
                        if (!ViewState.Ignored)
                        {
                            if (ViewState.ReservationNotificationTimeReached && !reservationTimeReached)
                            {
                                if (ViewState.DismissedTime.HasValue && ViewState.DismissedTime.Value.AddMinutes(1) <= DateTime.Now)
                                {
                                    //TODO: AAAAA ONLY IF NOT CONFIRMED OR NOT PAID
                                    _ = ShowNotification();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to update next host reservation.");
                }
                finally
                {
                    _reservationRefreshLock.Release();
                }
            }
        }

        protected override async Task OnInitializing(CancellationToken ct)
        {
            //_ = LoadNextHostReservation(); //Only for demo
            _gizmoClient.StartUp += OnStartUp;
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.ConnectionStateChange += OnConnectionStateChange;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;

            await base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            _gizmoClient.ConnectionStateChange -= OnConnectionStateChange;
            _gizmoClient.StartUp -= OnStartUp;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
        }

        private void OnConnectionStateChange(object? sender, ConnectionStateEventArgs e)
        {
            if (e.IsConnected)
            {
                _ = LoadNextHostReservation();
            }
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.LoggedIn:

                    break;

                case LoginState.LoggedOut:

                    ViewState.DismissedTime = null;
                    ViewState.Ignored = false;
                    ViewState.RaiseChanged();

                    break;
            }
        }

        private async void OnStartUp(object? sender, StartUpEventArgs e)
        {
            await LoadNextHostReservation();
        }

        public async Task ShowNotification()
        {
            if (await _reservationWarningLock.WaitAsync(TimeSpan.Zero))
            {
                _warningCancellationTokenSource = new CancellationTokenSource();

                try
                {
                    if (ViewState.DismissedTime.HasValue)
                    {
                        ViewState.DismissedTime = null;
                        DebounceViewStateChanged();
                    }

                    await _confirmReservationNotificationViewService.StartAsync(_warningCancellationTokenSource.Token);
                }
                catch { }
                finally
                {
                    _reservationWarningLock.Release();
                }

                _warningCancellationTokenSource.Dispose();
                _warningCancellationTokenSource = null;
            }
        }

        public async Task ShowDialog()
        {
            _confirmReservationNotificationViewService.CloseIfOpen();

            if (await _reservationWarningLock.WaitAsync(TimeSpan.Zero))
            {
                _warningCancellationTokenSource = new CancellationTokenSource();

                try
                {
                    if (ViewState.DismissedTime.HasValue)
                    {
                        ViewState.DismissedTime = null;
                        DebounceViewStateChanged();
                    }

                    StopTimer();

                    await _confirmReservationDialogViewService.StartAsync(_warningCancellationTokenSource.Token);

                    StartTimer();
                }
                catch { }
                finally
                {
                    _reservationWarningLock.Release();
                }

                _warningCancellationTokenSource.Dispose();
                _warningCancellationTokenSource = null;
            }
        }

        public void Ignore()
        {
            ViewState.DismissedTime = null;
            ViewState.Ignored = true;
            ViewState.RaiseChanged();
        }

        public void Dismiss()
        {
            ViewState.DismissedTime = DateTime.Now;
            ViewState.RaiseChanged();
        }

        private void OnAPIEventMessage(object? sender, Web.Api.Messaging.IAPIEventMessage e)
        {
            if (e is ReservationPaymentStatusChangedEvent reservationPaymentStatusChangedEvent)
            {
                if (reservationPaymentStatusChangedEvent.ReservationId == ViewState.ReservationId)
                {
                    ViewState.ReservationPaymentStatus = reservationPaymentStatusChangedEvent.Status;

                    if (ViewState.ReservationPaymentStatus == ReservationPaymentStatus.NotRequired ||
                        ViewState.ReservationPaymentStatus == ReservationPaymentStatus.Satisfied)
                    {
                        _confirmReservationDialogViewService.CloseIfWaitingPayment();
                    }

                    DebounceViewStateChanged();
                }
            }
            //else if (e is ReservationHostActivatedEventMessage reservationHostActivatedEventMessage)
            //{
            //    _ = LoadNextHostReservation();
            //}
            else if (e is ReservationHostAddedEventMessage reservationHostAddedEventMessage)
            {
                _ = LoadNextHostReservation();
            }
            else if (e is ReservationHostCancelledEventMessage reservationHostCancelledEventMessage)
            {
                _ = LoadNextHostReservation();
            }
            else if (e is ReservationHostCompletedEventMessage reservationHostCompletedEventMessage)
            {
                _ = LoadNextHostReservation();
            }
            else if (e is ReservationHostExpiredEventMessage reservationHostExpiredEventMessage)
            {
                _ = LoadNextHostReservation();
            }
            else if (e is ReservationHostUpdatedEventMessage reservationHostUpdatedEventMessage)
            {
                _ = LoadNextHostReservation();
            }
            else if (e is ReservationHostMovedEventMessage reservationHostMovedEventMessage)
            {
                _ = LoadNextHostReservation();
            }
        }

        private void StopTimer()
        {
            if (_reservationRefreshTimer != null)
            {
                _reservationRefreshTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _reservationRefreshTimer.Dispose();
                _reservationRefreshTimer = null;
            }
        }

        private void StartTimer()
        {
            StopTimer();

            //If there is a reservation then start the timer.
            if (_nextReservation != null)
            {
                _reservationRefreshTimer = new Timer(ReservationRefreshCallback, null, RESERVATION_REFFRESH_INTERVAL, RESERVATION_REFFRESH_INTERVAL);
            }
        }
    }
}
