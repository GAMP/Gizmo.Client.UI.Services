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
            ValidateProperty(() => ViewState.OldPassword);
            DebounceViewStateChanged();
        }

        public void SetNewPassword(string value)
        {
            ViewState.NewPassword = value;
            ValidateProperty(() => ViewState.NewPassword);
            DebounceViewStateChanged();
        }

        public void SetRepeatPassword(string value)
        {
            ViewState.RepeatPassword = value;
            ValidateProperty(() => ViewState.RepeatPassword);
            DebounceViewStateChanged();
        }

        public async Task SubmitAsync()
        {
            Validate();

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
