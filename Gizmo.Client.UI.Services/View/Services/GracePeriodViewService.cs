using System.Threading;
using System.Timers;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class GracePeriodViewService : ViewStateServiceBase<GracePeriodViewState>
    {
        public GracePeriodViewService(GracePeriodViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<GracePeriodViewService> logger,
            IServiceProvider serviceProvider)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;
        private System.Threading.Timer? _timer;

        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.IsInGracePeriod = _gizmoClient.IsInputLocked;
            DebounceViewStateChanged();

            _gizmoClient.GracePeriodChange += OnGracePeriodChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.GracePeriodChange -= OnGracePeriodChange;
            base.OnDisposing(isDisposing);
        }

        private void OnGracePeriodChange(object? sender, GracePeriodChangeEventArgs e)
        {
            ViewState.IsInGracePeriod = e.IsInGracePeriod;

            if (ViewState.IsInGracePeriod)
            {
                ViewState.Time = TimeSpan.FromMinutes(e.GracePeriodTime);

                _timer?.Dispose();
                _timer = new System.Threading.Timer(OnTimerCallback, null, 0, 1000);
            }
            else
            {
                _timer?.Dispose();
            }

            DebounceViewStateChanged();
        }

        private void OnTimerCallback(object? state)
        {
            ViewState.Time = TimeSpan.FromSeconds(ViewState.Time.TotalSeconds - 1);

            if (ViewState.Time.TotalSeconds <= 0)
            {
                ViewState.Time = TimeSpan.FromSeconds(0);

                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }

            DebounceViewStateChanged();
        }
    }
}
