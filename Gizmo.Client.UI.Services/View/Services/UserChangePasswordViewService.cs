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

        public async Task StartAsync(CancellationToken cToken = default)
        {
            await ResetAsync();

            var s = await _dialogService.ShowChangePasswordDialogAsync(cToken);
            if (s.Result == AddComponentResultCode.Opened)
                _ = await s.WaitForDialogResultAsync(cToken);
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
            ViewState.IsInitializing = false;
            ViewState.IsInitialized = null;
            ViewState.IsComplete = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.NewPassword) || fieldIdentifier.FieldEquals(() => ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    AddError(() => ViewState.RepeatPassword, _localizationService.GetString("PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }
    }
}
