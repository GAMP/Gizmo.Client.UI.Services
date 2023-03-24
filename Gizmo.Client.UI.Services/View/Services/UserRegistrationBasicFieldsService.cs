using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
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

        public void SetPassword(string value)
        {
            ViewState.Password = value;
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

        public void Clear()
        {
            ViewState.Username = string.Empty;
            ViewState.Password = string.Empty;
            ViewState.RepeatPassword = string.Empty;
            ViewState.FirstName = string.Empty;
            ViewState.LastName = string.Empty;
            ViewState.BirthDate = null;
            ViewState.Sex = Sex.Unspecified;
            ViewState.Email = string.Empty;
        }

        public async Task SubmitAsync()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            ViewState.IsValid = EditContext.Validate(); //TODO: AAA VALIDATE ASYNC?

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                return;
            }

            var userRegistrationIndexViewState = ServiceProvider.GetRequiredService<UserRegistrationIndexViewState>();

            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();
            var userRegistrationConfirmationMethodViewState = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewState>();

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != RegistrationVerificationMethod.None;
            bool confirmationWithMobilePhone = userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone; //TODO: A If both methods are available then get user selection.

            if (userRegistrationViewState.DefaultUserGroupRequiredInfo.Country ||
                userRegistrationViewState.DefaultUserGroupRequiredInfo.Address ||
                userRegistrationViewState.DefaultUserGroupRequiredInfo.PostCode ||
                (userRegistrationViewState.DefaultUserGroupRequiredInfo.Mobile && !confirmationWithMobilePhone))
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

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
                        Username = ViewState.Username,
                        FirstName = ViewState.FirstName,
                        LastName = ViewState.LastName,
                        BirthDate = ViewState.BirthDate,
                        Sex = ViewState.Sex
                    };

                    if (userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email)
                    {
                        profile.Email = userRegistrationConfirmationMethodViewState.Email;
                    }
                    else
                    {
                        profile.Email = ViewState.Email;
                    }

                    var userAgreements = userRegistrationIndexViewState.UserAgreementStates.Select(a => new UserAgreementModelState()
                    {
                        UserAgreementId = a.Id,
                        AcceptState = a.AcceptState
                    }).ToList();

                    if (!confirmationRequired)
                    {
                        var result = await _gizmoClient.UserCreateCompleteAsync(profile, ViewState.Password, userAgreements);

                        if (result.Result != AccountCreationCompleteResultCode.Success)
                        {
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("REGISTER_FAILED_MESSAGE");

                            return;
                        }
                    }
                    else
                    {
                        var result = await _gizmoClient.UserCreateByTokenCompleteAsync(userRegistrationConfirmationMethodViewState.Token, profile, ViewState.Password, userAgreements);

                        if (result.Result != AccountCreationByTokenCompleteResultCode.Success)
                        {
                            ViewState.HasError = true;
                            ViewState.ErrorMessage = _localizationService.GetString("REGISTER_FAILED_MESSAGE");

                            return;
                        }
                    }

                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "User create complete error.");

                    ViewState.HasError = true;
                    ViewState.ErrorMessage = ex.ToString();
                }
                finally
                {
                    ViewState.IsLoading = false;
                    ViewState.RaiseChanged();
                }
            }
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            await base.OnCustomValidationAsync(fieldIdentifier, validationMessageStore);

            if (fieldIdentifier.FieldName == nameof(ViewState.Username))
            {
                if (!string.IsNullOrEmpty(ViewState.Username))
                {
                    try
                    {
                        if (await _gizmoClient.UserExistAsync(ViewState.Username))
                        {
                            validationMessageStore.Add(() => ViewState.Username, _localizationService.GetString("VE_USERNAME_USED"));
                        }
                    }
                    catch (Exception ex)
                    {
                        validationMessageStore.Add(() => ViewState.Username, "Cannot validate username!"); //TODO: AAA TRANSLATE
                    }
                }
            }

            if (fieldIdentifier.FieldName == nameof(ViewState.Password) || fieldIdentifier.FieldName == nameof(ViewState.RepeatPassword))
            {
                if (!string.IsNullOrEmpty(ViewState.Password) && !string.IsNullOrEmpty(ViewState.RepeatPassword) && string.Compare(ViewState.Password, ViewState.RepeatPassword) != 0)
                {
                    validationMessageStore.Add(() => ViewState.RepeatPassword, _localizationService.GetString("PASSWORDS_DO_NOT_MATCH"));
                }
            }
        }

        #endregion
    }
}
