using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Route(ClientRoutes.LoginRoute)]
    [Register()]
    public sealed class UserLoginService : ValidatingViewStateServiceBase<UserLoginViewState>, IDisposable
    {
        public UserLoginService(UserLoginViewState viewState,
            ILogger<UserLoginService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        public void SetLoginMethod(UserLoginType userLoginType)
        {
            ViewState.LoginType = userLoginType;
            DebounceViewStateChanged();
        }

        public void SetLoginName(string value)
        {
            ViewState.LoginName = value;
            ValidateProperty(() => ViewState.LoginName);
            DebounceViewStateChanged();
        }

        public void SetPassword(string value)
        {
            ViewState.Password = value;
            ValidateProperty(() => ViewState.Password);
            DebounceViewStateChanged();
        }

        public void SetPasswordVisible(bool value)
        {
            ViewState.IsPasswordVisible = value;
            DebounceViewStateChanged();
        }

        public Task<bool> UsernameCharacterIsValid(char value)
        {
            return Task.FromResult(value != '!');
        }

        public async Task LoginAsync()
        {
            //always validate state on submission
            EditContext.Validate();

            //model validation is pending, we cant proceed
            if (ViewState.IsValid != true)
                return;

            string? loginName = ViewState.LoginName;
            string? password = ViewState.Password;

            if (string.IsNullOrEmpty(loginName) || string.IsNullOrEmpty(password))
                return;

            try
            {
                var result = await _gizmoClient.UserLoginAsync(loginName, password);
                Logger.LogTrace("Client login result {result}", result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated client login error.");
            }
        }

        public Task OpenRegistrationAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.RegistrationIndexRoute);
            return Task.CompletedTask;
        }

        public Task LogοutAsync()
        {
            NavigationService.NavigateTo(ClientRoutes.HomeRoute);
            return Task.CompletedTask;
        }

        public void Reset()
        {
            ViewState.SetDefaults();
            ResetValidationState();
            DebounceViewStateChanged();
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            //whenever we move away from login page we should make full view state reset
            Reset();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnUserLoginStateChange;
            _gizmoClient.UserIdleChange += OnSystemUserIdleChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool dis)
        {
            _gizmoClient.LoginStateChange -= OnUserLoginStateChange;
            _gizmoClient.UserIdleChange -= OnSystemUserIdleChange;
            base.OnDisposing(dis);
        }

        private void OnSystemUserIdleChange(object? sender, UserIdleEventArgs e)
        {
            //once user becomes idle we need to clear any input made into login form
            if (e.IsIdle)
            {
                Reset();
            }
        }

        private void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            switch (e.State)
            {
                case LoginState.LoginFailed:
                    switch (e.FailReason)
                    {
                        //only clear password input in case of invalid password
                        case LoginResult.InvalidPassword:
                            ViewState.Password = null;
                            break;
                        //clera both username and pasword in any other error case
                        default:
                            ViewState.LoginName = null;
                            ViewState.Password = null;
                            break;
                    }

                    //process login error reason
                    ViewState.HasLoginError = true;
                    ViewState.LoginError = e.FailReason.ToString();
                    break;
                case LoginState.LoggedIn:
                    Reset();
                    break;
                default:
                    break;
            }

            switch (e.State)
            {
                //LoggingIn state is the only statye
                case LoginState.LoggingIn:
                    ViewState.IsLogginIn = true;
                    break;
                default:
                    ViewState.IsLogginIn = false;
                    break;
            }

            DebounceViewStateChanged();
        }

        protected override Task OnValidateAsync(FieldIdentifier fieldIdentifier, CancellationToken cancellationToken = default)
        {
            return base.OnValidateAsync(fieldIdentifier, cancellationToken);
        }
    }
}
