using Gizmo.Client.UI.Services;
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
    public sealed class UserChangePasswordViewService : ValidatingViewStateServiceBase<UserChangePasswordViewState>
    {
        #region CONSTRUCTOR
        public UserChangePasswordViewService(UserChangePasswordViewState viewState,
            ILogger<UserChangePasswordViewService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IClientDialogService dialogService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _dialogService = dialogService;
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IClientDialogService _dialogService;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS

        public void SetOldPassword(string value)
        {
            ViewState.OldPassword = value;
            ValidateProperty(() => ViewState.OldPassword);
        }

        public void SetNewPassword(string value)
        {
            ViewState.NewPassword = value;
            ValidateProperty(() => ViewState.NewPassword);
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ValidateProperty(() => ViewState.RepeatPassword);
        }

        public async Task StartAsync(bool showOldPassword, CancellationToken cToken = default)
        {
            try
            {
                await ResetAsync();

                ViewState.ShowOldPassword = showOldPassword;

                var s = await _dialogService.ShowChangePasswordDialogAsync(cToken);
                if (s.Result == AddComponentResultCode.Opened)
                    _ = await s.WaitForResultAsync(cToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to update user password.");
            }
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                await _gizmoClient.UserPasswordUpdateAsync(ViewState.OldPassword, ViewState.NewPassword);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User password update error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = ex.ToString();
            }
            finally
            {
                ViewState.IsComplete = true;
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public Task ResetAsync()
        {
            ViewState.ShowOldPassword = false;
            ViewState.OldPassword = string.Empty;
            ViewState.NewPassword = string.Empty;
            ViewState.RepeatPassword = string.Empty;

            ViewState.IsInitializing = false;
            ViewState.IsInitialized = null;
            ViewState.IsComplete = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (ViewState.ShowOldPassword && fieldIdentifier.FieldEquals(() => ViewState.OldPassword) && string.IsNullOrEmpty(ViewState.OldPassword))
            {
                AddError(() => ViewState.OldPassword, _localizationService.GetString("GIZ_USER_CHANGE_PASSWORD_VE_OLD_PASSWORD_IS_REQUIRED"));
            }
            if (fieldIdentifier.FieldEquals(() => ViewState.NewPassword) || fieldIdentifier.FieldEquals(() => ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    AddError(() => ViewState.RepeatPassword, _localizationService.GetString("GIZ_GEN_PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }
    }
}
