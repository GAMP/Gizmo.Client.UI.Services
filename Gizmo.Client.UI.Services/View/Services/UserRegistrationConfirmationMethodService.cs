using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components.Forms;
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
            ILocalizationService localizationService,
            IGizmoClient gizmoClient,
            UserRegistrationViewState userRegistrationViewState) : base(viewState, logger, serviceProvider)
        {
            _localizationService = localizationService;

            _gizmoClient = gizmoClient;
            _userRegistrationViewState = userRegistrationViewState;

            _timer.Elapsed += timer_Elapsed;
        }
        #endregion

        #region FIELDS
        private readonly ILocalizationService _localizationService;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private System.Timers.Timer _timer = new System.Timers.Timer(1000);
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ViewState.RaiseChanged();
        }

        public void SetCountry(string value)
        {
            ViewState.Country = value;
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

        public void Clear()
        {
            ViewState.Email = null;
            ViewState.Country = null;
            ViewState.Prefix = null;
            ViewState.MobilePhone = null;
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
                if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email)
                {
                    var result = await _gizmoClient.UserCreateByEmailStartAsync(ViewState.Email);

                    if (result.Result != VerificationStartResultCode.Success)
                    {
                        //TODO: A HANDLE ERROR
                    }

                    ViewState.Token = result.Token;
                }
                else if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone)
                {
                    var result = await _gizmoClient.UserCreateByMobileStartAsync(ViewState.MobilePhone);

                    if (result.Result != VerificationStartResultCode.Success)
                    {
                        //TODO: A HANDLE ERROR
                    }

                    ViewState.Token = result.Token;
                }

                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                ViewState.CanResend = false;
                ViewState.ResendTimeLeft = TimeSpan.FromMinutes(5);
                _timer.Start();

                NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);

                ViewState.RaiseChanged();
            }
            catch
            {
                //TODO: A HANDLE ERROR
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

        protected override async Task OnCustomValidationAsync(FieldIdentifier fieldIdentifier, ValidationMessageStore validationMessageStore)
        {
            await base.OnCustomValidationAsync(fieldIdentifier, validationMessageStore);

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.Email &&
                fieldIdentifier.FieldName == nameof(ViewState.Email))
            {
                if (string.IsNullOrEmpty(ViewState.Email))
                {
                    validationMessageStore.Add(() => ViewState.Email, _localizationService.GetString("EMAIL_IS_REQUIRED"));
                }
                else
                {
                    //TODO: A VALIDATE EMAIL FORMAT
                    if (await _gizmoClient.UserEmailExistAsync(ViewState.Email))
                    {
                        validationMessageStore.Add(() => ViewState.Email, _localizationService.GetString("EMAIL_IS_IN_USE"));
                    }
                }
            }

            if (_userRegistrationViewState.ConfirmationMethod == RegistrationVerificationMethod.MobilePhone &&
                fieldIdentifier.FieldName == nameof(ViewState.MobilePhone))
            {
                if (string.IsNullOrEmpty(ViewState.MobilePhone))
                {
                    validationMessageStore.Add(() => ViewState.MobilePhone, _localizationService.GetString("PHONE_IS_REQUIRED"));
                }
                else
                {
                    //TODO: A VALIDATE PHONE FORMAT?
                    if (await _gizmoClient.UserMobileExistAsync(ViewState.MobilePhone))
                    {
                        validationMessageStore.Add(() => ViewState.MobilePhone, _localizationService.GetString("PHONE_IS_IN_USE"));
                    }
                }
            }
        }

        #endregion
    }
}
