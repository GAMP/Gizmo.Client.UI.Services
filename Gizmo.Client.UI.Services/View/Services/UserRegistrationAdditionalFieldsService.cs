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

            bool confirmationRequired = true; //TODO: A

            try
            {
                if (!confirmationRequired)
                {
                    string password = string.Empty; //TODO: A

                    await _gizmoClient.UserCreateCompleteAsync(new Web.Api.Models.UserModelUpdate()
                    {

                    }, password);
                }
                else
                {
                    string token = string.Empty; //TODO: A DON'T WE NEED CONFIRMATION CODE AGAIN?
                    string password = string.Empty; //TODO: A

                    await _gizmoClient.UserCreateByTokenCompleteAsync(token, new Web.Api.Models.UserModelUpdate()
                    {

                    }, password);
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

        protected override async Task OnNavigatedIn()
        {
            await base.OnNavigatedIn();

            ViewState.DefaultUserGroupRequiredInfo = await _gizmoClient.UserGroupDefaultRequiredInfoGetAsync() ?? new();
        }

        #endregion
    }
}
