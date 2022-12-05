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
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        #endregion

        #region FUNCTIONS

        public void SetLoginMethod(UserLoginType userLoginType)
        {
            ViewState.LoginType = userLoginType;

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
                //simulate login task
                await Task.Delay(1000);

                //initiate login
                if (ViewState.LoginName == "1")
                {
                    //Failed login
                    ViewState.IsLogginIn = false;
                    ViewState.HasLoginError = true;
                    ViewState.RaiseChanged();

                    //Reset after some time?
                    await Task.Delay(10000);

                    ViewState.SetDefaults();
                    ResetValidationErrors();

                    ViewState.RaiseChanged();
                }
                else
                {
                    //Successful login
                    NavigationService.NavigateTo(ClientRoutes.HomeRoute);

                    await LoadUserProfileAsync();
                    await LoadUserBalanceAsync();
                    await ShowUserAgreementsAsync();

                    //Cleanup
                    ViewState.SetDefaults();
                    ResetValidationErrors();

                    ViewState.RaiseChanged();
                }
            }
            catch
            {

            }
            finally
            {

            }
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

        private Task LoadUserProfileAsync()
        {
            //TODO: A
            var userViewState = ServiceProvider.GetRequiredService<UserViewState>();

            userViewState.Username = "Test Username";
            userViewState.FirstName = "Test First Name";
            userViewState.LastName = "Test Last Name";
            userViewState.BirthDate = new DateTime(1950, 1, 2);
            userViewState.Sex = Sex.Male;
            userViewState.Country = "Greece";
            userViewState.Address = "Test Address 123";
            userViewState.Email = "test@test.test";
            userViewState.Phone = "0123456789";
            userViewState.MobilePhone = "1234567890";
            userViewState.RegistrationDate = new DateTime(2020, 3, 4);
            userViewState.Picture = "_content/Gizmo.Client.UI/img/Cyber_Punk.png";

            userViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        private Task LoadUserBalanceAsync()
        {
            var userBalanceViewState = ServiceProvider.GetRequiredService<UserBalanceViewState>();

            userBalanceViewState.Balance = 10.76m;
            userBalanceViewState.CurrentTimeProduct = "Six Hours (6) for 10$ Pack";
            userBalanceViewState.Time = new TimeSpan(6, 36, 59);
            userBalanceViewState.PointsBalance = 416;

            userBalanceViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        private async Task ShowUserAgreementsAsync()
        {
            var userAgreementsService = ServiceProvider.GetRequiredService<UserAgreementsService>();
            await userAgreementsService.LoadUserAgreementsAsync(1); //TODO: A USER ID

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