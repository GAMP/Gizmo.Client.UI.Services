using Gizmo.Client.UI.Services.View.Constants;
using Gizmo.Client.UI.View.Constants;
using Gizmo.Client.UI.View.States;
using Gizmo.Shared.Client.Enumerations;
using Gizmo.Shared.Client.Options;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class HostNumberViewService : ViewStateServiceBase<HostNumberViewState>
    {
        #region CONSTRUCTOR
        public HostNumberViewService(HostNumberViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<HostNumberViewService> logger,
            IServiceProvider serviceProvider, IOptionsMonitor<HostNumberOptions> hostNumberOptions, 
            IOptionsMonitor<LoginRotatorOptions> loginRotatorOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _logger = logger;
            _hostNumberOptions = hostNumberOptions;
            _loginRotatorOptions = loginRotatorOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IOptionsMonitor<HostNumberOptions> _hostNumberOptions;
        private readonly IOptionsMonitor<LoginRotatorOptions> _loginRotatorOptions;
        private readonly ILogger<HostNumberViewService> _logger;
        private List<HostNumberFixedPosition> Positions { get; set; }
        private int _currentIndex = -1;
        private Timer _timer;
        private Timer _animationTimer;
        public event Action OnPositionChanged;
        #endregion

        
        protected override Task OnInitializing(CancellationToken ct)
        {
            ViewState.HostNumber = _gizmoClient.Number;
            ViewState.Prefix = _hostNumberOptions.CurrentValue.Prefix;
            ViewState.PrefixIsEnabled = !_hostNumberOptions.CurrentValue.PrefixDisabled;
            ViewState.AnimationDuration = _hostNumberOptions.CurrentValue.AnimationDuration;
            ViewState.HostNumberIsEnabled = _loginRotatorOptions.CurrentValue.Enabled 
                ? _hostNumberOptions.CurrentValue.ShowOnRotator
                : _hostNumberOptions.CurrentValue.ShowOnWallpaper;

            InitHostNumberBehaviour();
            DebounceViewStateChanged();
            return base.OnInitializing(ct);
        }

        private void InitHostNumberBehaviour()
        {
            var hostNumberOptionsValue = _hostNumberOptions.CurrentValue;
            if (hostNumberOptionsValue is { ShowOnWallpaper: false, ShowOnRotator: false })
                return;
            
            var positioning = _loginRotatorOptions.CurrentValue.Enabled
                ? hostNumberOptionsValue.PositioningOnRotator
                : hostNumberOptionsValue.PositioningOnWallpaper;
            
            if (positioning == HostNumberPositioning.Fixed)
                ViewState.CurrentPosition = hostNumberOptionsValue.FixedPosition.ToStringValue();
            else
            {
                Positions = GetMovementPositions(positioning);

                var allTimeDuration = Positions.Count * (hostNumberOptionsValue.TimeoutBetweenPosition +
                    hostNumberOptionsValue.AnimationDuration);
                var countOfCircles = Math.Floor(3600 / allTimeDuration);
                
                List<TimeSpan> startTimes = new List<TimeSpan>();
                var currentTimeSeconds = 0d;
                
                for (var i = 0; i < countOfCircles; i++)
                {
                    startTimes.Add(TimeSpan.FromSeconds(currentTimeSeconds));
                    currentTimeSeconds += allTimeDuration;
                }

                var positionTimeout = TimeSpan.FromSeconds(hostNumberOptionsValue.TimeoutBetweenPosition);
                _timer = new Timer(ChangePosition, null, positionTimeout, positionTimeout);
            }
        }

        private void ChangePosition(object? state)
        {
            var isInitCircle = _currentIndex == -1;
            _currentIndex = isInitCircle ? 0 : _currentIndex;
            
            _animationTimer = new Timer(_ =>
                {
                    _currentIndex = (_currentIndex + 1) % Positions.Count;
                    ViewState.CurrentPosition = Positions[_currentIndex].ToStringValue();
                    OnPositionChanged?.Invoke();
                    _animationTimer?.Dispose();
                }, null, TimeSpan.FromSeconds(!isInitCircle ? _hostNumberOptions.CurrentValue.AnimationDuration : 0), 
                Timeout.InfiniteTimeSpan);
        }

        private List<HostNumberFixedPosition> GetMovementPositions(HostNumberPositioning positioning)
        {
            var positions = new List<HostNumberFixedPosition>();
            
            switch (positioning)
            {
                case HostNumberPositioning.Clockwise: 
                    positions = HostNumberPositioningLists.Clockwise;
                    break;
                case HostNumberPositioning.Counterclockwise: 
                    positions = HostNumberPositioningLists.CounterClockwise;
                    break;
                case HostNumberPositioning.HorizontalTop: 
                    positions = HostNumberPositioningLists.HorizontalTop;
                    break;
                case HostNumberPositioning.HorizontalCenter: 
                    positions = HostNumberPositioningLists.HorizontalCenter;
                    break;
                case HostNumberPositioning.HorizontalBottom: 
                    positions = HostNumberPositioningLists.HorizontalBottom;
                    break;
                case HostNumberPositioning.VerticalLeft: 
                    positions = HostNumberPositioningLists.VerticalLeft;
                    break;
                case HostNumberPositioning.VerticalCenter: 
                    positions = HostNumberPositioningLists.VerticalCenter;
                    break;
                case HostNumberPositioning.VerticalRight: 
                    positions = HostNumberPositioningLists.VerticalRight;
                    break;
                case HostNumberPositioning.WallBounce: 
                    positions = HostNumberPositioningLists.WallBounce;
                    break;
                default:
                    _logger.LogWarning("Unknown positioning. A fixed host number position will be used");
                    break;
            }

            return positions;
        }
    }
}
