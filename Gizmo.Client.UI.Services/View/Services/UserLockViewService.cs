using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLockViewService : ValidatingViewStateServiceBase<UserLockViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLockViewService(UserLockViewState viewState,
            IGizmoClient gizmoClient,
            ILogger<UserLockViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS

        public void SetInputPassword(string value)
        {
            ViewState.InputPassword = value;
            DebounceViewStateChanged();
        }

        public Task<bool> SetInputPasswordCharacterIsValid(char value)
        {
            return Task.FromResult(char.IsNumber(value) && ViewState.InputPassword.Length < 4);
        }

        public async Task LockAsync()
        {
            //go into full screen mode
            await _gizmoClient.EnterFullSceenAsync();

            ViewState.IsLocking = true;
            ViewState.RaiseChanged();            
        }

        public async Task CancelLockAsync()
        {
            //Do not allow cancel if it's already locked.
            if (ViewState.IsLocking && !ViewState.IsLocked)
            {
                //exit full screen mode
                await _gizmoClient.ExitFullSceenAsync();

                ViewState.IsLocking = false;
                ViewState.InputPassword = string.Empty;
                ViewState.RaiseChanged();             
            }
        }

        public Task SetPasswordAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            if (ViewState.InputPassword.Length == 4)
            {
                _gizmoClient.UserLockEnterAsync();

                ViewState.IsLocking = false;
                ViewState.IsLocked = true;

                ViewState.LockPassword = ViewState.InputPassword;
                ViewState.InputPassword = string.Empty;
            }
            else
            {
                //TODO: A Errors
            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public async Task UnlockAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            if (ViewState.InputPassword == ViewState.LockPassword)
            {
                //exit user lock mode
                await _gizmoClient.UserLockExitAsync();

                ViewState.IsLocked = false;
                ViewState.InputPassword = string.Empty;
                ViewState.LockPassword = string.Empty;
            }
            else
            {
                ViewState.Error = _localizationService.GetString("GIZ_USER_LOCK_SCREEN_INCORRECT_PIN");
            }

            ViewState.RaiseChanged();
        }

        public Task PutDigitAsync(int number)
        {
            if (ViewState.InputPassword.Length < 4)
            {
                ViewState.InputPassword += number.ToString();

                ViewState.RaiseChanged();
            }

            return Task.CompletedTask;
        }

        public Task DeleteDigitAsync()
        {
            if (ViewState.InputPassword.Length > 0)
            {
                ViewState.InputPassword = ViewState.InputPassword.Substring(0, ViewState.InputPassword.Length - 1);

                ViewState.RaiseChanged();
            }

            return Task.CompletedTask;
        }

        #endregion

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggingOut)
            {
                try
                {
                    //exit user lock mode
                    await _gizmoClient.UserLockExitAsync();

                    ViewState.IsLocked = false;
                    ViewState.InputPassword = string.Empty;
                    ViewState.LockPassword = string.Empty;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to reset user lock.");
                }
            }

            DebounceViewStateChanged();
        }

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.InputPassword))
            {
                if (ViewState.InputPassword.Length != 4)
                {
                    AddError(() => ViewState.InputPassword, _localizationService.GetString("GIZ_USER_LOCK_SCREEN_VE_PASSWORD_LENGTH"));
                }
            }
        }
    }
}
