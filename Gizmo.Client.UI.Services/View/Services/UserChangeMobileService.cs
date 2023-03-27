using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserChangeMobileService : ValidatingViewStateServiceBase<UserChangeMobileViewState>
    {
        #region CONSTRUCTOR
        public UserChangeMobileService(UserChangeMobileViewState viewState,
            ILogger<UserChangeMobileService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _timer.Elapsed += timer_Elapsed;
        }
        #endregion

        #region FIELDS

        private System.Timers.Timer _timer = new System.Timers.Timer(1000);

        #endregion

        #region FUNCTIONS

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
            DebounceViewStateChanged();
        }

        public void SetPrefix(string value)
        {
            ViewState.Prefix = value;
            DebounceViewStateChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
            DebounceViewStateChanged();
        }

        public void SetConfirmationCode(string value)
        {
            ViewState.ConfirmationCode = value;
            ValidateProperty(() => ViewState.ConfirmationCode);
            DebounceViewStateChanged();
        }

        public async Task SendConfirmationCodeAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                // Simulate task.
                await Task.Delay(2000);

                ViewState.CanResend = false;
                ViewState.ResendTimeLeft = TimeSpan.FromMinutes(5);
                _timer.Start();

                ViewState.IsLoading = false;

                ViewState.PageIndex = 1;
                ViewState.RaiseChanged();
            }
            catch
            {

            }
            finally
            {

            }
        }

        private void timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ViewState.ResendTimeLeft = ViewState.ResendTimeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (ViewState.ResendTimeLeft.TotalSeconds == 0)
            {
                ViewState.CanResend = true;

                _timer.Stop();
            }

            ViewState.RaiseChanged();
        }

        public Task VerifyAsync()
        {
            ViewState.PageIndex = 2;
            ViewState.IsComplete = true;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task ResendAsync()
        {
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task ResetAsync()
        {
            ViewState.PageIndex = 0;
            ViewState.IsComplete = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (ViewState.PageIndex == 1 && fieldIdentifier.FieldEquals(() => ViewState.ConfirmationCode) && string.IsNullOrEmpty(ViewState.ConfirmationCode))
            {
                AddError(() => ViewState.ConfirmationCode, "The confirmatin code field is required.");
            }
        }

        #endregion
    }
}
