using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserChangePasswordService : ValidatingViewStateServiceBase<UserChangePasswordViewState>
    {
        #region CONSTRUCTOR
        public UserChangePasswordService(UserChangePasswordViewState viewState,
            ILogger<UserChangePasswordService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS

        public void SetOldPassword(string value)
        {
            ViewState.OldPassword = value;
            ViewState.RaiseChanged();
        }

        public void SetNewPassword(string value)
        {
            ViewState.NewPassword = value;
            ViewState.RaiseChanged();
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            await _gizmoClient.UserPasswordUpdateAsync(ViewState.OldPassword, ViewState.NewPassword);


            //TODO: A UPDATE PASSWORD
            ViewState.IsComplete = true;

            ViewState.RaiseChanged();
        }

        public Task ResetAsync()
        {
            ViewState.IsComplete = false;

            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.NewPassword) || fieldIdentifier.FieldName == nameof(ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    validationMessageStore.Add(() => ViewState.RepeatPassword, _localizationService.GetString("PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }
    }
}
