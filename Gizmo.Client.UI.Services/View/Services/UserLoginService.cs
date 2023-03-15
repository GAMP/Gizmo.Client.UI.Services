using System.Threading;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserLoginService : ValidatingViewStateServiceBase<UserLoginViewState>, IDisposable
    {
        #region CONSTRUCTOR
        public UserLoginService(UserLoginViewState viewState,
            ILogger<UserLoginService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        #endregion

        #region FUNCTIONS

        public void SetLoginMethod(UserLoginType userLoginType)
        {
            ViewState.LoginType = userLoginType;
            ViewState.RaiseChanged();
        }

        public Task<bool> UsernameCharacterIsValid(char value)
        {
            return Task.FromResult(value != '!');
        }

        public void SetLoginName(string value)
        {
            ViewState.LoginName = value;
            ViewState.RaiseChanged();
        }

        public void SetPassword(string value)
        {
            ViewState.Password = value;
            ViewState.RaiseChanged();
        }

        public void SetPasswordVisible(bool value)
        {
            ViewState.IsPasswordVisible = value;
            ViewState.RaiseChanged();
        }

        public async Task LoginAsync()
        {
            //always validate state on submission
            ViewState.IsValid = EditContext.Validate();

            //model validation is pending, we cant proceed
            if (ViewState.IsValid != true)
                return;

            ViewState.IsLogginIn = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.UserLoginAsync(ViewState.LoginName, ViewState.Password);
                if (result == LoginResult.Sucess)
                    NavigationService.NavigateTo(ClientRoutes.HomeRoute);
            }
            catch
            {

            }
            finally
            {
                ViewState.IsLogginIn = false;
            }

            DebounceViewStateChange();
        }

        public Task OpenRegistrationAsync()
        {
            var userRegistrationConfirmationMethodService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodService>();
            return userRegistrationConfirmationMethodService.StartRegistrationAsync();
        }

        public Task LogοutAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.HomeRoute);
            return Task.CompletedTask;
        }

        private async Task LoadUserProfileAsync(CancellationToken cancellationToken = default)
        {
            var userProfile = await _gizmoClient.UserProfileGetAsync(cancellationToken);

            var userViewState = ServiceProvider.GetRequiredService<UserViewState>();

            userViewState.Id = userProfile.Id;
            userViewState.Username = userProfile.Username;
            userViewState.FirstName = userProfile.FirstName;
            userViewState.LastName = userProfile.LastName;
            userViewState.BirthDate = userProfile.BirthDate;
            userViewState.Sex = userProfile.Sex;
            userViewState.Country = userProfile.Country;
            userViewState.Address = userProfile.Address;
            userViewState.Email = userProfile.Email;
            userViewState.Phone = userProfile.Phone;
            userViewState.MobilePhone = userProfile.MobilePhone;
            //TODO: A USER PICTURE
            //userViewState.RegistrationDate = userProfile.RegistrationDate;
            userViewState.Picture = "_content/Gizmo.Client.UI/img/Cyber_Punk.png";

            userViewState.RaiseChanged();
        }

        private async Task LoadUserBalanceAsync(CancellationToken cancellationToken = default)
        {
            var userBalance = await _gizmoClient.UserBalanceGetAsync(cancellationToken);
            //TODO: A UPDATE FROM USERBALANCE
            var userBalanceViewState = ServiceProvider.GetRequiredService<UserBalanceViewState>();

            userBalanceViewState.Balance = 10.76m;
            userBalanceViewState.CurrentTimeProduct = "#Six Hours (6) for 10$ Pack";
            userBalanceViewState.Time = new TimeSpan(6, 36, 59);
            userBalanceViewState.PointsBalance = 416;

            userBalanceViewState.RaiseChanged();
        }

        private async Task ShowUserAgreementsAsync()
        {
            var userAgreementsService = ServiceProvider.GetRequiredService<UserAgreementsService>();
            await userAgreementsService.LoadUserAgreementsAsync();

            while (userAgreementsService.ViewState.HasUserAgreements)
            {
                var s = await _dialogService.ShowUserAgreementDialogAsync();
                if (s.Result == DialogAddResult.Success)
                {
                    try
                    {
                        var result = await s.WaitForDialogResultAsync();
                        await userAgreementsService.AcceptCurrentUserAgreementAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        //TODO: A CLEANER SOLUTION?
                        if (userAgreementsService.ViewState.CurrentUserAgreement.IsRejectable)
                        {
                            await userAgreementsService.RejectCurrentUserAgreementAsync();
                        }
                        else
                        {
                            //TODO: IF REJECTED CLEANUP AND LOGOUT?
                            await userAgreementsService.RejectCurrentUserAgreementAsync();
                            return;
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            ViewState.SetDefaults();
            ResetValidationErrors();

            ViewState.RaiseChanged();
        }

        #endregion
    }
}
