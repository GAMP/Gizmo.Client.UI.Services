using System.Diagnostics.Metrics;
using System.Net;
using System.Numerics;
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

            //Fill UserRegistrationViewState
            var userRegistrationViewState = ServiceProvider.GetRequiredService<UserRegistrationViewState>();

            if (userRegistrationViewState.ConfirmationMethod != UserRegistrationMethod.MobilePhone)
            {
                userRegistrationViewState.Country = ViewState.Country;
                userRegistrationViewState.MobilePhone = ViewState.MobilePhone;
            }

            userRegistrationViewState.Address = ViewState.Address;
            userRegistrationViewState.PostCode = ViewState.PostCode;

            bool confirmationRequired = userRegistrationViewState.ConfirmationMethod != UserRegistrationMethod.None;

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

        #endregion

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
