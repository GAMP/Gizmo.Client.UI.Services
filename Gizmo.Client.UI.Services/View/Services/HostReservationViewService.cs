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
        private readonly SemaphoreSlim _notificationLock = new(1);
        private readonly SemaphoreSlim _dialogLock = new(1);
        private Timer? _reservationRefreshTimer = null;
        private const int RESERVATION_REFFRESH_INTERVAL = 1000;

        private CancellationTokenSource? _notificationCancellationTokenSource = null;
        private CancellationTokenSource? _dialogCancellationTokenSource = null;
        private CancellationTokenSource? _cancellationTokenSource;
        private const int DebounceDelay = 500;

        private async Task LoadNextHostReservation()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                StopTimer();

                await Task.Delay(DebounceDelay, _cancellationTokenSource.Token);
                _nextReservation = await _gizmoClient.ClientReservationGetAsync(_cancellationTokenSource.Token);

                await ReservationRefresh();

                StartTimer();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load next host reservation.");
            }
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
                    var reservationTime = _nextReservation?.NextReservationTime == null ? _nextReservation?.NextReservationTime : _nextReservation.NextReservationTime.Value.ToLocalTime();
                    var reservationBlockTime = _nextReservation?.LoginBlockBeforeTime;
                    var reservationNotificationTime = _reservationOptions.CurrentValue.AlertBeforeTime;
                    var reservationPaymentStatus = _nextReservation?.PaymentStatus;
                    var reservationDuration = _nextReservation?.Duration;
                    var reservationTotal = _nextReservation?.Total;
                    var reservationOutstanding = _nextReservation?.Outstanding;
                    var reservationHosts = _nextReservation?.Hosts;

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

                    var previousReservationHostIds = ViewState.Hosts.Select(a => a.HostNumber).ToList();
                    var reservationHostIds = reservationHosts?.Select(a => a.HostNumber).ToList() ?? [];

                    //Update UI only if there are changes.
                    if (ViewState.ReservationId != reservationId ||
                        ViewState.Time != reservationTime ||
                        ViewState.ReservationTimeReached != reservationTimeReached ||
                        ViewState.ReservationBlockTimeReached != reservationBlockTimeReached ||
                        ViewState.ReservationNotificationTimeReached != reservationNotificationTimeReached ||
                        ViewState.ReservationPaymentStatus != reservationPaymentStatus ||
                        ViewState.Duration != reservationDuration ||
                        ViewState.Total != reservationTotal ||
                        ViewState.Outstanding != reservationOutstanding ||
                        !previousReservationHostIds.SequenceEqual(reservationHostIds))
                    {
                        bool important = false;
                        if (ViewState.ReservationId != reservationId ||
                            ViewState.Time != reservationTime ||
                            ViewState.ReservationTimeReached != reservationTimeReached ||
                            ViewState.ReservationBlockTimeReached != reservationBlockTimeReached ||
                            ViewState.ReservationNotificationTimeReached != reservationNotificationTimeReached ||
                            ViewState.ReservationPaymentStatus != reservationPaymentStatus)
                        {
                            important = true;
                        }

                        if (ViewState.ReservationId != reservationId)
                        {
                            ViewState.Ignored = false;
                            ViewState.DismissedTime = null;
                        }

                        var previousReservationTimeReached = ViewState.ReservationTimeReached;
                        var previousReservationBlockTimeReached = ViewState.ReservationBlockTimeReached;
                        var previousReservationNotificationTimeReached = ViewState.ReservationNotificationTimeReached;
                        var previousReservationPaymentStatus = ViewState.ReservationPaymentStatus;

                        ViewState.ReservationId = reservationId;
                        ViewState.Time = reservationTime;
                        ViewState.ReservationTimeReached = reservationTimeReached;
                        ViewState.ReservationBlockTimeReached = reservationBlockTimeReached;
                        ViewState.ReservationNotificationTimeReached = reservationNotificationTimeReached;
                        ViewState.ReservationPaymentStatus = reservationPaymentStatus;
                        ViewState.Duration = reservationDuration;
                        ViewState.Total = reservationTotal;
                        ViewState.Outstanding = reservationOutstanding;
                        ViewState.Hosts = reservationHosts?.Select(a => new ReservationInfoHostModel()
                        {
                            HostNumber = a.HostNumber,
                            HostName = a.HostName
                        }) ?? [];

                        if ((previousReservationPaymentStatus != ReservationPaymentStatus.NotRequired || previousReservationPaymentStatus != ReservationPaymentStatus.Satisfied) &&
                            reservationPaymentStatus == ReservationPaymentStatus.NotRequired || reservationPaymentStatus == ReservationPaymentStatus.Satisfied)
                        {
                            if (ViewState.Ignored)
                            {
                                var result = await _gizmoClient.ReservationCurrentConfirmedAsync();

                                if (result == ReservationCurrentConfirmedResult.Confirmed)
                                {
                                    ResetIgnore();
                                }
                            }
                        }

                        DebounceViewStateChanged();

                        if (important)
                        {
                            if (ViewState.ReservationId.HasValue)
                            {
                                if (_gizmoClient.IsUserLoggedIn)
                                {
                                    if (reservationTimeReached)
                                    {
                                        if (!previousReservationTimeReached)
                                        {
                                            //If not confirmed will be logged out. //TODO: AAAAA CHECK THAT THE DIALOG WILL BE CLOSED IN THIS CASE.

                                            _ = ShowDialog();
                                        }
                                    }
                                    else
                                    {
                                        if (!ViewState.Ignored)
                                        {
                                            if (reservationNotificationTime.HasValue)
                                            {
                                                if (!previousReservationNotificationTimeReached && reservationNotificationTimeReached)
                                                {
                                                    _ = ShowNotification();
                                                }
                                            }
                                            else if (reservationBlockTime.HasValue)
                                            {
                                                //The notification time is not set but block time is set.
                                                if (!previousReservationBlockTimeReached && reservationBlockTimeReached)
                                                {
                                                    //If not confirmed will be logged out. //TODO: AAAAA CHECK THAT THE DIALOG WILL BE CLOSED IN THIS CASE.

                                                    //Show notification for payment
                                                    _ = ShowNotification();
                                                }
                                            }
                                        }

                                        if (!reservationNotificationTimeReached && !reservationBlockTimeReached)
                                        {
                                            //Reservation was moved to the future. Close any warning.
                                            if (_notificationCancellationTokenSource != null)
                                            {
                                                _notificationCancellationTokenSource.Cancel();
                                            }
                                            if (_dialogCancellationTokenSource != null)
                                            {
                                                _dialogCancellationTokenSource.Cancel();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //There is no reservation anymore. Close any warning and stop timer.
                                if (_notificationCancellationTokenSource != null)
                                {
                                    _notificationCancellationTokenSource.Cancel();
                                }
                                if (_dialogCancellationTokenSource != null)
                                {
                                    _dialogCancellationTokenSource.Cancel();
                                }

                                StopTimer();
                                return;
                            }
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
            _gizmoClient.StartUp += OnStartUp;
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;

            await base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            _gizmoClient.StartUp -= OnStartUp;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
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
            if (await _notificationLock.WaitAsync(TimeSpan.Zero))
            {
                _notificationCancellationTokenSource = new CancellationTokenSource();

                try
                {
                    if (ViewState.DismissedTime.HasValue)
                    {
                        ViewState.DismissedTime = null;
                        DebounceViewStateChanged();
                    }

                    await _confirmReservationNotificationViewService.StartAsync(_notificationCancellationTokenSource.Token);
                }
                catch { }
                finally
                {
                    _notificationLock.Release();
                }

                _notificationCancellationTokenSource.Dispose();
                _notificationCancellationTokenSource = null;
            }
        }

        public async Task ShowDialog()
        {
            if (await _dialogLock.WaitAsync(TimeSpan.Zero))
            {
                //Close notification if open.
                if (_notificationCancellationTokenSource != null)
                {
                    _notificationCancellationTokenSource.Cancel();
                }

                _dialogCancellationTokenSource = new CancellationTokenSource();

                try
                {
                    if (ViewState.DismissedTime.HasValue)
                    {
                        ViewState.DismissedTime = null;
                        DebounceViewStateChanged();
                    }

                    await _confirmReservationDialogViewService.StartAsync(_dialogCancellationTokenSource.Token);
                }
                catch { }
                finally
                {
                    _dialogLock.Release();
                }

                _dialogCancellationTokenSource.Dispose();
                _dialogCancellationTokenSource = null;
            }
        }

        public void Ignore()
        {
            ViewState.DismissedTime = null;
            ViewState.Ignored = true;
            ViewState.RaiseChanged();
        }

        public void ResetIgnore()
        {
            ViewState.DismissedTime = null;
            ViewState.Ignored = false;
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
                    if (reservationPaymentStatusChangedEvent.Status == ReservationPaymentStatus.NotRequired ||
                        reservationPaymentStatusChangedEvent.Status == ReservationPaymentStatus.Satisfied)
                    {
                        _confirmReservationNotificationViewService.CloseIfWaitingPayment();
                        _confirmReservationDialogViewService.ShowPaymentSuccessful();
                    }

                    _ = LoadNextHostReservation();
                }
            }

            // TODO : Since we only going to be receiving only specific reservation events atm we could simply check reservation event by base type like this       
            //if (e is Web.Api.Messaging.ReservationEventMessageBase)
            //{

            //}
            // or with explicit function that check for desired event like IsExpectedEvent(IAPIEventMessage e)
            // the other thing is debouncing, since in some cases 2-3 events might occur almost at same time and we will generate multiple calls to server

            // TODO : Looks like payment status change event is not accounted for here, this causes the current _nextReservation value to remain in same unpaid state
            // even when we have processed the payment and closed the payment dialog


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
