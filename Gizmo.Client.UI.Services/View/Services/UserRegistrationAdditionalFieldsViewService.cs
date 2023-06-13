using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationAdditionalFieldsRoute)]
    public sealed class UserRegistrationAdditionalFieldsViewService : ValidatingViewStateServiceBase<UserRegistrationAdditionalFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationAdditionalFieldsViewService(UserRegistrationAdditionalFieldsViewState viewState,
            ILogger<UserRegistrationAdditionalFieldsViewService> logger,
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

        public void SetAddress(string value)
        {
            ViewState.Address = value;
            ValidateProperty(() => ViewState.Address);
        }

        public void SetPostCode(string value)
        {
            ViewState.PostCode = value;
            ValidateProperty(() => ViewState.PostCode);
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
            ValidateProperty(() => ViewState.Country);
        }

        public void SetPrefix(string value)
        {
            ViewState.Prefix = value;
            DebounceViewStateChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ValidateProperty(() => ViewState.MobilePhone);
        }

        public void Clear()
        {
            ViewState.Address = null;
            ViewState.PostCode = null;
            ViewState.Country = null;
            ViewState.Prefix = null;
            ViewState.MobilePhone = null;

            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;

            ResetValidationState();
            DebounceViewStateChanged();
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            var userRegistrationIndexViewState = ServiceProvider.GetRequiredService<UserRegistrationIndexViewState>();

            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();
            var userRegistrationConfirmationMethodViewState = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewState>();
            var userRegistrationBasicFieldsViewState = ServiceProvider.GetRequiredService<UserRegistrationBasicFieldsViewState>();

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != RegistrationVerificationMethod.None;

            try
            {
                var profile = new Web.Api.Models.UserProfileModelCreate()
                {
                    Username = userRegistrationBasicFieldsViewState.Username,
                    FirstName = userRegistrationBasicFieldsViewState.FirstName,
                    LastName = userRegistrationBasicFieldsViewState.LastName,
                    BirthDate = userRegistrationBasicFieldsViewState.BirthDate,
                    Sex = userRegistrationBasicFieldsViewState.Sex,
                    Email = userRegistrationBasicFieldsViewState.Email,
                    Address = ViewState.Address,
                    PostCode = ViewState.PostCode
                };
                
                if (userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone)
                {
                    profile.Country = userRegistrationConfirmationMethodViewState.Country;

                    var tmp = userRegistrationConfirmationMethodViewState.Prefix + userRegistrationConfirmationMethodViewState.MobilePhone;
                    if (tmp.StartsWith("+"))
                    {
                        tmp = tmp.Substring(1);
                    }

                    profile.MobilePhone = tmp;
                }
                else
                {
                    profile.Country = ViewState.Country;

                    var tmp = ViewState.Prefix + ViewState.MobilePhone;
                    if (tmp.StartsWith("+"))
                    {
                        tmp = tmp.Substring(1);
                    }

                    profile.MobilePhone = tmp;
                }

                var userAgreements = userRegistrationIndexViewState.UserAgreementStates.Select(a => new UserAgreementModelState()
                {
                    UserAgreementId = a.Id,
                    AcceptState = a.AcceptState
                }).ToList();

                if (!confirmationRequired)
                {
                    var password = userRegistrationBasicFieldsViewState.Password;

                    var result = await _gizmoClient.UserCreateCompleteAsync(profile, password, userAgreements);

                    if (result.Result != AccountCreationCompleteResultCode.Success)
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString("GIZ_REGISTRATION_FAILED_MESSAGE");

                        return;
                    }
                }
                else
                {
                    var token = userRegistrationConfirmationMethodViewState.Token;
                    var password = userRegistrationBasicFieldsViewState.Password;

                    var result = await _gizmoClient.UserCreateByTokenCompleteAsync(token, profile, password, userAgreements);

                    if (result.Result != AccountCreationByTokenCompleteResultCode.Success)
                    {
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString("GIZ_REGISTRATION_FAILED_MESSAGE");

                        return;
                    }
                }

                //TODO: AAA SUCCESS MESSAGE?
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

        #endregion
    }
}
