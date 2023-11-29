using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserBanViewService : ViewStateServiceBase<UserBanViewState>
    {
        public UserBanViewService(UserBanViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserBanViewService> logger,
            IServiceProvider serviceProvider,
            UserViewService userViewService)
            : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _userViewService = userViewService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly UserViewService _userViewService;
        private System.Threading.Timer? _timer;

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        private void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is UserEnabledChangedEventMessage userEnabledChangedEventMessage)
            {
                if (userEnabledChangedEventMessage.Disabled)
                {
                    ViewState.IsDisabled = true;
                    ViewState.EnableDate = userEnabledChangedEventMessage.EnableDate;
                    ViewState.DisabledDate = userEnabledChangedEventMessage.DisabledDate;
                    ViewState.Reason = userEnabledChangedEventMessage.Reason;

                    ViewState.Time = TimeSpan.FromSeconds(10);

                    _timer?.Dispose();
                    _timer = new System.Threading.Timer(OnTimerCallback, null, 0, 1000);

                    DebounceViewStateChanged();
                }
            }
        }

        private void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedIn)
            {
                ViewState.IsDisabled = false;
                ViewState.EnableDate = null;
                ViewState.DisabledDate = null;
                ViewState.Reason = null;
            }
            else if (e.State == LoginState.LoggedOut)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        private async void OnTimerCallback(object? state)
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

                try
                {
                    await _userViewService.LogoutAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error while logging out banned user.");
                }
            }

            DebounceViewStateChanged();
        }

    }
}
