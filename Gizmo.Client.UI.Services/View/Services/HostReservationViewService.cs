using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
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

        private async Task LoadNextHostReservation()
        {
            try
            {
                var currentData = await _gizmoClient.NextHostReservationGetAsync();

                var reservationId = currentData?.NextReservationId;
                var reservationTime = currentData?.NextReservationTime;

                //check if we have reservation configuration data and that there is a reservation upcoming
                if (reservationId != null && reservationTime != null)
                {
                    var currentTime = DateTime.Now;

                    ViewState.Time = reservationTime;
                    ViewState.IsReserved = DateTime.Now.AddHours(1) >= reservationTime;

                    //if (currentData.EnableLoginBlock)
                    //{
                    //    var blockTime = reservationTime.Value.AddMinutes(currentData.LoginBlockTime * -1);

                    //    if (currentData.EnableLoginUnblock)
                    //    {
                    //        var unblockTime = reservationTime.Value.AddMinutes(currentData.LoginUnblockTime);
                    //        ViewState.IsLoginBlocked = currentTime <= unblockTime;
                    //    }
                    //    else
                    //    {
                    //        ViewState.IsLoginBlocked = currentTime >= blockTime;
                    //    }
                    //}
                    //else
                    //{
                    //    ViewState.IsLoginBlocked = false;
                    //}
                }
                else
                {
                    //in case no reservation data is present reset configuration

                    ViewState.IsReserved = false;
                    ViewState.IsLoginBlocked = false;
                    ViewState.Time = null;
                }

                DebounceViewStateChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to get the next host reservation.");
            }
        }

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await LoadNextHostReservation();

            _gizmoClient.ConnectionStateChange += OnConnectionStateChange;
            _gizmoClient.ReservationChange += OnReservationChange;
            await base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.ConnectionStateChange -= OnConnectionStateChange;
            _gizmoClient.ReservationChange -= OnReservationChange;

            base.OnDisposing(isDisposing);
        }

        private async void OnConnectionStateChange(object? sender, ConnectionStateEventArgs e)
        {
            await LoadNextHostReservation();
        }

        private async void OnReservationChange(object? sender, ReservationChangeEventArgs e)
        {
            await LoadNextHostReservation();
        }
    }
}
