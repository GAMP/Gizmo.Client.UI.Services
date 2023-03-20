using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationAdditionalFieldsRoute)]
    public sealed class UserRegistrationAdditionalFieldsService : ValidatingViewStateServiceBase<UserRegistrationAdditionalFieldsViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationAdditionalFieldsService(UserRegistrationAdditionalFieldsViewState viewState,
            ILogger<UserRegistrationAdditionalFieldsService> logger,
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

        public void SetAddress(string value)
        {
            ViewState.Address = value;
            ViewState.RaiseChanged();
        }

        public void SetPostCode(string value)
        {
            ViewState.PostCode = value;
            ViewState.RaiseChanged();
        }

        public void SetPrefix(string value)
        {
            ViewState.Prefix = value;
            ViewState.RaiseChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ViewState.RaiseChanged();
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            var userRegistrationIndexViewState = ServiceProvider.GetRequiredService<UserRegistrationIndexViewState>();

            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();
            var userRegistrationConfirmationMethodViewState = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewState>();
            var userRegistrationBasicFieldsViewState = ServiceProvider.GetRequiredService<UserRegistrationBasicFieldsViewState>();

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != UserRegistrationMethod.None;

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
                
                if (userRegistrationViewState.ConfirmationMethod == UserRegistrationMethod.MobilePhone)
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

                if (!confirmationRequired)
                {
                    var password = userRegistrationBasicFieldsViewState.Password;
                    var userAgreements = userRegistrationIndexViewState.UserAgreementStates.ToList();

                    var result = await _gizmoClient.UserCreateCompleteAsync(profile, password, userAgreements);

                    if (result.Result != AccountCreationCompleteResultCode.Success)
                    {
                        //TODO: A HANDLE ERROR
                    }
                }
                else
                {
                    var token = userRegistrationViewState.Token;
                    var password = userRegistrationBasicFieldsViewState.Password;
                    var userAgreements = userRegistrationIndexViewState.UserAgreementStates.ToList();

                    var result = await _gizmoClient.UserCreateByTokenCompleteAsync(token, profile, password, userAgreements);

                    if (result.Result != AccountCreationByTokenCompleteResultCode.Success)
                    {
                        //TODO: A HANDLE ERROR
                    }
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

        #endregion
    }
}
