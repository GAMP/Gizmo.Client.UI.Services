using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        public void SetFirstName(string value)
        {
            ViewState.FirstName = value;
            ViewState.RaiseChanged();
        }

        public void SetLastName(string value)
        {
            ViewState.LastName = value;
            ViewState.RaiseChanged();
        }

        public void SetBirthDate(DateTime? value)
        {
            ViewState.BirthDate = value;
            ViewState.RaiseChanged();
        }

        public void SetSex(Sex value)
        {
            ViewState.Sex = value;
            ViewState.RaiseChanged();
        }

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            //Fill UserRegistrationViewState
            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();

            if (userRegistrationViewState.ConfirmationMethod != UserRegistrationMethod.Email)
            {
                userRegistrationViewState.Email = ViewState.Email;
            }

            userRegistrationViewState.Username = ViewState.Username;
            userRegistrationViewState.FirstName = ViewState.FirstName;
            userRegistrationViewState.LastName = ViewState.LastName;
            userRegistrationViewState.BirthDate = ViewState.BirthDate;
            userRegistrationViewState.Sex = ViewState.Sex;

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != UserRegistrationMethod.None;
            bool confirmationWithMobilePhone = userRegistrationViewState.ConfirmationMethod == UserRegistrationMethod.MobilePhone; //TODO: A If both methods are available then get user selection.

            if (userRegistrationViewState.DefaultUserGroupRequiredInfo.Country ||
                userRegistrationViewState.DefaultUserGroupRequiredInfo.Address ||
                userRegistrationViewState.DefaultUserGroupRequiredInfo.PostCode ||
                (userRegistrationViewState.DefaultUserGroupRequiredInfo.Mobile && !confirmationWithMobilePhone))
            {
                //If any of the additional fields is required open the next page.
                NavigationService.NavigateTo(ClientRoutes.RegistrationAdditionalFieldsRoute);
            }
            else
            {
                //If no additional fields are required then proceed with sign up.

                try
                {
                    var profile = new Web.Api.Models.UserProfileModelCreate()
                    {
                        Username = userRegistrationViewState.Username,
                        FirstName = userRegistrationViewState.FirstName,
                        LastName = userRegistrationViewState.LastName,
                        BirthDate = userRegistrationViewState.BirthDate,
                        Sex = userRegistrationViewState.Sex,
                        Email = userRegistrationViewState.Email,
                        Country = userRegistrationViewState.Country,
                        MobilePhone = userRegistrationViewState.MobilePhone,
                        Address = userRegistrationViewState.Address,
                        PostCode = userRegistrationViewState.PostCode,
                    };

                    if (!confirmationRequired)
                    {
                        await _gizmoClient.UserCreateCompleteAsync(profile, userRegistrationViewState.Password, userRegistrationViewState.UserAgreementStates.ToList());
                    }
                    else
                    {
                        await _gizmoClient.UserCreateByTokenCompleteAsync(userRegistrationViewState.Token, profile, userRegistrationViewState.Password, userRegistrationViewState.UserAgreementStates.ToList());
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

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
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
                        validationMessageStore.Add(() => ViewState.Username, _localizationService.GetString("USERNAME_IS_IN_USE"));
                    }
                }
            }

            if (fieldIdentifier.FieldName == nameof(ViewState.NewPassword) || fieldIdentifier.FieldName == nameof(ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.NewPassword) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.NewPassword, ViewState.RepeatPassword) != 0)
                {
                    validationMessageStore.Add(() => ViewState.RepeatPassword, _localizationService.GetString("PASSWORDS_DO_NOT_MATCH"));
                }
            }

            //TODO: A VALIDATE EMAIL FORMAT
        }

        #endregion
    }
}
