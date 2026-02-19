using Gizmo.Client.Options;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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
            ConfirmReservationNotificationViewService confirmReservationNotificationViewService)
            : base(viewState, logger, serviceProvider)
        {
            _reservationOptions = reservationOptions;
            _gizmoClient = gizmoClient;
            _confirmReservationNotificationViewService = confirmReservationNotificationViewService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly ConfirmReservationNotificationViewService _confirmReservationNotificationViewService;

        private readonly IOptionsMonitor<ClientReservationOptions> _reservationOptions;
        private ClientNextReservationModel? _nextReservation;

        private readonly SemaphoreSlim _reservationRefreshLock = new(1);
        private Timer? _reservationRefreshTimer;
        private const int RESERVATION_REFFRESH_INTERVAL = 1000;

        public void SetConfirmed()
        {
            ViewState.IsConfirmed = true;
            DebounceViewStateChanged();
        }

        private async Task LoadNextHostReservation()
        {
            try
            {
                _reservationRefreshTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                _nextReservation = await _gizmoClient.ClientReservationGetAsync();

                await ReservationRefresh();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load next host reservation.");
            }

            //If there is a reservation then start the timer.
            if (_nextReservation != null)
            {
                _reservationRefreshTimer ??= new Timer(ReservationRefreshCallback);
                _reservationRefreshTimer.Change(RESERVATION_REFFRESH_INTERVAL, RESERVATION_REFFRESH_INTERVAL);
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
                    var reservationId = _nextReservation?.NextReservationId;
                    var reservationTime = _nextReservation?.NextReservationTime;
                    var reservationDuration = _nextReservation?.Duration;
                    var reservationBlockTime = _nextReservation?.LoginBlockBeforeTime;
                    var reservationNotificationTime = _reservationOptions.CurrentValue.AlertBeforeTime;
                    var reservationPaymentStatus = _nextReservation?.PaymentStatus;

                    var reservationBlockTimeReached = false;
                    var reservationNotificationTimeReached = false;
                    DateTime? time = null;

                    //check if we have reservation configuration data and that there is a reservation upcoming
                    if (reservationId != null && reservationTime != null && reservationDuration != null)
                    {
                        var currentTime = DateTime.Now;

                        time = reservationTime;

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
                        ViewState.Time != time ||
                        ViewState.ReservationBlockTimeReached != reservationBlockTimeReached ||
                        ViewState.ReservationNotificationTimeReached != reservationNotificationTimeReached ||
                        ViewState.ReservationPaymentStatus != reservationPaymentStatus)
                    {
                        var previousReservationBlockTimeReached = ViewState.ReservationBlockTimeReached;
                        var previousReservationNotificationTimeReached = ViewState.ReservationNotificationTimeReached;

                        ViewState.ReservationId = reservationId;
                        ViewState.Time = time;
                        ViewState.ReservationBlockTimeReached = reservationBlockTimeReached;
                        ViewState.ReservationNotificationTimeReached = reservationNotificationTimeReached;
                        ViewState.ReservationPaymentStatus = reservationPaymentStatus;

                        DebounceViewStateChanged();

                        if (reservationNotificationTime.HasValue)
                        {
                            if (!previousReservationNotificationTimeReached && reservationNotificationTimeReached)
                            {
                                if (_gizmoClient.IsUserLoggedIn)
                                {
                                    await _confirmReservationNotificationViewService.StartAsync();
                                }
                            }
                        }
                        else if (reservationBlockTime.HasValue)
                        {
                            //The notification time is not set but block time is set.
                            if (!previousReservationBlockTimeReached && reservationBlockTimeReached)
                            {
                                if (_gizmoClient.IsUserLoggedIn)
                                {
                                    //TODO: AAAAA IF LOGGED IN USER IS CONFIRMED THEN SHOW NOTIFICATION FOR PAYMENT?
                                    await _confirmReservationNotificationViewService.StartAsync();
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
            _gizmoClient.ReservationChange += OnReservationChange;
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            await base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.StartUp -= OnStartUp;
            _gizmoClient.ReservationChange -= OnReservationChange;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;

            base.OnDisposing(isDisposing);
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.LoggedIn:

                    //_reservationRefreshTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                    break;

                case LoginState.LoggedOut:

                    ViewState.IsConfirmed = false;

                    //If there is a reservation then start the timer.
                    if (_nextReservation != null)
                    {
                        _reservationRefreshTimer ??= new Timer(ReservationRefreshCallback);
                        _reservationRefreshTimer.Change(RESERVATION_REFFRESH_INTERVAL, RESERVATION_REFFRESH_INTERVAL);
                    }

                    break;
            }
        }

        private async void OnStartUp(object? sender, StartUpEventArgs e)
        {
            await LoadNextHostReservation();
        }

        private async void OnReservationChange(object? sender, ReservationChangeEventArgs e)
        {
            await LoadNextHostReservation();
        }
    }
}
