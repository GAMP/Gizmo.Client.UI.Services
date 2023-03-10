using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Web;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationBasicFieldsRoute)]
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

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            bool confirmationRequired = true; //TODO: A
            bool confirmationWithMobilePhone = true;

            if (ViewState.DefaultUserGroupRequiredInfo.Country ||
                ViewState.DefaultUserGroupRequiredInfo.Address ||
                ViewState.DefaultUserGroupRequiredInfo.PostCode ||
                (ViewState.DefaultUserGroupRequiredInfo.Mobile && !confirmationWithMobilePhone))
            {
                //If any of the additional fields is required open the next page.
                NavigationService.NavigateTo(ClientRoutes.RegistrationAdditionalFieldsRoute);
            }
            else
            {
                //If no additional fields are required then proceed with sign up.

                try
                {
                    if (!confirmationRequired)
                    {
                        string password = string.Empty; //TODO: A

                        await _gizmoClient.UserCreateCompleteAsync(new Web.Api.Models.UserProfileModelCreate()
                        {

                        }, password, new List<Web.Api.Models.UserAgreementModelState>());
                    }
                    else
                    {
                        string token = string.Empty; //TODO: A DON'T WE NEED CONFIRMATION CODE AGAIN?
                        string password = string.Empty; //TODO: A

                        await _gizmoClient.UserCreateByTokenCompleteAsync(token, new Web.Api.Models.UserProfileModelCreate()
                        {

                        }, password, new List<Web.Api.Models.UserAgreementModelState>());
                    }

                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                }
                catch
                {
                    //TODO: A HANDLE ERROR
                }
                finally
                {

                }
            }
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.DefaultUserGroupRequiredInfo = await _gizmoClient.UserGroupDefaultRequiredInfoGetAsync() ?? new();
        }

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username))
                {
                    if (await _gizmoClient.UserExistAsync(ViewState.Username))
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
