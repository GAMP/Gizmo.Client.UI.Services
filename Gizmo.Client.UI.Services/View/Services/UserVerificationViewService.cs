using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.PasswordRecoveryRoute)]
    public sealed class UserVerificationViewService : ViewStateServiceBase<UserVerificationViewState>
    {
        private const int VERIFICATION_DELAY = 60;

        #region CONTRUCTOR
        public UserVerificationViewService(UserVerificationViewState viewState,
            ILogger<UserVerificationViewService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _timer.Elapsed += timer_Elapsed;
        }

        #endregion

        #region FIELDS
        private readonly SemaphoreSlim verificationLock = new SemaphoreSlim(1, 1);
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(1000);
        #endregion

        #region FUNCTIONS

        internal async Task<bool> LockAsync()
        {
            if (await verificationLock.WaitAsync(-1))
            {
                var previouslyLocked = ViewState.IsVerificationLocked;

                if (!previouslyLocked)
                {
                    ViewState.IsVerificationLocked = true;
                    try
                    {
                        ViewState.RaiseChanged();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to raise changed on UserVerificationViewState.");
                    }
                }

                verificationLock.Release();
                return !previouslyLocked;
            }

            return false;
        }

        internal async Task Unlock()
        {
            _timer.Stop();

            if (await verificationLock.WaitAsync(-1))
            {
                ViewState.IsVerificationLocked = false;
                try
                {
                    ViewState.RaiseChanged();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to raise changed on UserVerificationViewState.");
                }

                verificationLock.Release();
            }
        }

        internal void StartUnlockTimer()
        {
            ViewState.Countdown = TimeSpan.FromSeconds(VERIFICATION_DELAY);
            ViewState.RaiseChanged();
            _timer.Start();
        }

        #endregion

        private async void timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ViewState.Countdown = ViewState.Countdown.Subtract(TimeSpan.FromSeconds(1));

            if (ViewState.Countdown.TotalSeconds <= 0)
            {
                ViewState.Countdown = TimeSpan.FromSeconds(0);
                await Unlock();
            }

            ViewState.RaiseChanged();
        }
    }
}
