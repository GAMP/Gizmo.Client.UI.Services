using System.Threading;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;
        private ClientReservationOptions? _configuration;
        private NextHostReservationModel? _currentData;

        private readonly SemaphoreSlim _reservationRefreshLock = new(1);
        private Timer? _reservationRefreshTimer;
        private const int RESERVATION_REFFRESH_INTERVAL = 1000;

        private bool _requiresRefresh = false;

        private async Task LoadNextHostReservation()
        {
            try
            {
                _reservationRefreshTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                _configuration = await _gizmoClient.ReservationConfigurationGetAsync();
                _currentData = await _gizmoClient.NextHostReservationGetAsync();

                _requiresRefresh = false;

                await ReservationRefresh();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load next host reservation.");
            }

            //If there is a reservation then start the timer.
            if (_currentData != null)
            {
                _reservationRefreshTimer ??= new Timer(ReservationRefreshCallback);
                _reservationRefreshTimer.Change(RESERVATION_REFFRESH_INTERVAL, RESERVATION_REFFRESH_INTERVAL);
            }
        }

        private async void ReservationRefreshCallback(object? state)
        {
            if (_requiresRefresh)
            {
                await LoadNextHostReservation();
            }

            await ReservationRefresh();
        }

        private async Task ReservationRefresh()
        {
            if (await _reservationRefreshLock.WaitAsync(TimeSpan.Zero))
            {
                try
                {
                    var reservationId = _currentData?.NextReservationId;
                    var reservationTime = _currentData?.NextReservationTime;
                    var reservationDuration = _currentData?.NextReservationDuration;

                    bool isReserved;
                    bool isLoginBlocked;
                    DateTime? time;

                    //check if we have reservation configuration data and that there is a reservation upcoming
                    if (_configuration != null && reservationId != null && reservationTime != null && reservationDuration != null)
                    {
                        var currentTime = DateTime.Now;

                        if (currentTime > reservationTime.Value.AddMinutes(reservationDuration.Value))
                        {
                            //In case of expired reservation reset configuration.

                            isReserved = false;
                            isLoginBlocked = false;
                            time = null;

                            _requiresRefresh = true;
                        }
                        else
                        {
                            time = reservationTime;
                            isReserved = DateTime.Now.AddHours(1) >= reservationTime;

                            if (_configuration.EnableLoginBlock)
                            {
                                var blockTime = reservationTime.Value.AddMinutes(_configuration.LoginBlockTime * -1);

                                if (currentTime >= blockTime)
                                {
                                    isLoginBlocked = true;

                                    if (_configuration.EnableLoginUnblock)
                                    {
                                        var unblockTime = reservationTime.Value.AddMinutes(_configuration.LoginUnblockTime);
                                        isLoginBlocked = currentTime <= unblockTime;
                                    }
                                }
                                else
                                {
                                    isLoginBlocked = false;
                                }
                            }
                            else
                            {
                                isLoginBlocked = false;
                            }
                        }
                    }
                    else
                    {
                        //in case no reservation data is present reset configuration

                        isReserved = false;
                        isLoginBlocked = false;
                        time = null;
                    }

                    //Update UI only if there are changes.
                    if (ViewState.IsReserved != isReserved || ViewState.IsLoginBlocked != isLoginBlocked || ViewState.Time != time)
                    {
                        ViewState.IsReserved = isReserved;
                        ViewState.IsLoginBlocked = isLoginBlocked;
                        ViewState.Time = time;

                        DebounceViewStateChanged();
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

                    _reservationRefreshTimer?.Change(Timeout.Infinite, Timeout.Infinite);

                    break;

                case LoginState.LoggedOut:

                    //If there is a reservation then start the timer.
                    if (_currentData != null)
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
