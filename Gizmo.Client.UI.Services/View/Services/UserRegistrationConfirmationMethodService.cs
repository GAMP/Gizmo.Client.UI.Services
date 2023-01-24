using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserRegistrationConfirmationMethodService : ValidatingViewStateServiceBase<UserRegistrationConfirmationMethodViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationConfirmationMethodService(UserRegistrationConfirmationMethodViewState viewState,
            ILogger<UserRegistrationConfirmationMethodService> logger,
            IServiceProvider serviceProvider,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            ViewState.ConfirmationMethod = UserRegistrationMethod.MobilePhone;
            _dialogService = dialogService;

            _timer.Elapsed += timer_Elapsed;
        }
        #endregion

        #region FIELDS
        private readonly IClientDialogService _dialogService;
        private System.Timers.Timer _timer = new System.Timers.Timer(1000);
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
        }

        public void SetMobilePhone(string value)
        {
            ViewState.MobilePhone = value;
            ViewState.RaiseChanged();
        }

        public async Task StartRegistrationAsync()
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
                            //TODO: IF REJECTED CLEANUP AND RETURN
                            await userAgreementsService.RejectCurrentUserAgreementAsync();
                            return;
                        }
                    }
                }
            }

            if (ViewState.ConfirmationMethod == UserRegistrationMethod.None)
            {
                NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
            }
            else
            {
                NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationMethodRoute);
            }
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                ViewState.CanResend = false;
                ViewState.ResendTimeLeft = TimeSpan.FromMinutes(5);
                _timer.Start();

                NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);

                //TODO: A
                await Task.Delay(5000);
                ViewState.RaiseChanged();
            }
            catch
            {

            }
            finally
            {

            }
        }

        private void timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            ViewState.ResendTimeLeft = ViewState.ResendTimeLeft.Subtract(TimeSpan.FromSeconds(1));

            if (ViewState.ResendTimeLeft.TotalSeconds == 0)
            {
                ViewState.CanResend = true;

                _timer.Stop();
            }

            ViewState.RaiseChanged();
        }

        #endregion

        #region OVERRIDES

        protected override void OnCustomValidation(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            base.OnCustomValidation(fieldIdentifier, validationMessageStore);

            if (ViewState.ConfirmationMethod == UserRegistrationMethod.Email &&
                fieldIdentifier.FieldName == nameof(ViewState.Email) &&
                string.IsNullOrEmpty(ViewState.Email))
            {
                validationMessageStore.Add(() => ViewState.Email, "The e-mail field is required.");
            }
            

            if (ViewState.ConfirmationMethod == UserRegistrationMethod.MobilePhone &&
                fieldIdentifier.FieldName == nameof(ViewState.MobilePhone) &&
                string.IsNullOrEmpty(ViewState.MobilePhone))
            {
                validationMessageStore.Add(() => ViewState.MobilePhone, "The phone number field is required.");
            }
        }

        #endregion
    }
}
