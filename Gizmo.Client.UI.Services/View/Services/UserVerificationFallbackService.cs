using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class UserVerificationFallbackService : ViewStateServiceBase<UserVerificationFallbackViewState>
    {
        private const int FALLBACK_DELAY = 30;

        #region CONTRUCTOR
        public UserVerificationFallbackService(UserVerificationFallbackViewState viewState,
            ILogger<UserVerificationFallbackService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _timer.Elapsed += timer_Elapsed;
        }
        #endregion

        #region FIELDS
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(1000);
        #endregion

        #region FUNCTIONS

        internal void SetSMSFallbackAvailability(bool value)
        {
            ViewState.IsSMSFallbackAvailable = value;
            ViewState.RaiseChanged();
        }

        internal void Lock()
        {
            ViewState.IsVerificationFallbackLocked = true;
            ViewState.RaiseChanged();
        }

        internal void StartUnlockTimer()
        {
            ViewState.Countdown = TimeSpan.FromSeconds(FALLBACK_DELAY);
            ViewState.RaiseChanged();

            _timer.Start();
        }

        internal void Unlock()
        {
            _timer.Stop();
            ViewState.IsVerificationFallbackLocked = false;
            ViewState.RaiseChanged();
        }

        #endregion

        private void timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ViewState.Countdown = ViewState.Countdown.Subtract(TimeSpan.FromSeconds(1));

            if (ViewState.Countdown.TotalSeconds <= 0)
            {
                ViewState.Countdown = TimeSpan.FromSeconds(0);
                Unlock();
            }

            ViewState.RaiseChanged();
        }
    }
}
