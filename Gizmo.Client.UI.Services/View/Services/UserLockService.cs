using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLockService : ValidatingViewStateServiceBase<UserLockViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLockService(UserLockViewState viewState,
            ILogger<UserLockService> logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
        }
        #endregion

        #region FUNCTIONS

        public void SetInputPassword(string value)
        {
            ViewState.InputPassword = value;
            DebounceViewStateChanged();
        }

        public Task LockAsync()
        {
            ViewState.IsLocking = true;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task CancelLockAsync()
        {
            //Do not allow cancel if it's already locked.
            if (ViewState.IsLocking && !ViewState.IsLocked)
            {
                ViewState.IsLocking = false;

                ViewState.InputPassword = string.Empty;

                ViewState.RaiseChanged();
            }

            return Task.CompletedTask;
        }

        public Task SetPasswordAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            if (ViewState.InputPassword.Length == 4)
            {
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

        public Task UnlockAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            if (ViewState.InputPassword == ViewState.LockPassword)
            {
                ViewState.IsLocked = false;

                ViewState.InputPassword = string.Empty;
                ViewState.LockPassword = string.Empty;
            }
            else
            {
                ViewState.Error = "Incorrect PIN"; //TODO: A TRANSLATE
            }

            ViewState.RaiseChanged();

            return Task.CompletedTask;
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

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.InputPassword))
            {
                if (ViewState.InputPassword.Length != 4)
                {
                    AddError(() => ViewState.InputPassword, "Password should have 4 digits!");
                }
            }
        }
    }
}
