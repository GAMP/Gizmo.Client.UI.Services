using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationBasicFieldsService : ValidatingViewStateServiceBase<UserRegistrationBasicFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationBasicFieldsService(UserRegistrationBasicFieldsViewState viewState,
            ILogger<UserRegistrationBasicFieldsService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region FUNCTIONS

        public void SetUsername(string value)
        {
            ViewState.Username = value;
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

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            //TODO: A CHECK IF WE HAVE REQUIRED ADDITIONAL FIELDS BEFORE GO THERE.
            NavigationService.NavigateTo(ClientRoutes.RegistrationAdditionalFieldsRoute);
            return Task.CompletedTask;
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            //TODO: A
            ViewState.DefaultUserGroupRequiredInfo = await _gizmoClient.GetDefaultUserGroupRequiredInfoAsync();
        }

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username))
                {
                    if (await _gizmoClient.UsernameExistAsync(ViewState.Username))
                    {
                        validationMessageStore.Add(() => ViewState.Username, "The Username is in use."); //TODO: A TRANSLATE
                    }
                }
            }

            if (fieldIdentifier.FieldName == nameof(ViewState.NewPassword) || fieldIdentifier.FieldName == nameof(ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    validationMessageStore.Add(() => ViewState.RepeatPassword, "Passwords do not match!"); //TODO: A TRANSLATE
                }
            }
        }

        #endregion
    }
}