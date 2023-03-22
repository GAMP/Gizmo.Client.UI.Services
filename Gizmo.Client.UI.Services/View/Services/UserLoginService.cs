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
            using (ViewStateChangeDebounced())
            {
                ViewState.LoginType = userLoginType;
            }
        }

        public void SetLoginName(string value)
        {
            using (ViewStateChangeDebounced())
            {            
                ViewState.LoginName = value;
                ValidateProperty((x) => x.LoginName);
            }
        }

        public void SetPassword(string value)
        {
            using (ViewStateChangeDebounced())
            {
                ViewState.Password = value;
                ValidateProperty((x) => x.Password);
            }
        }

        public void SetPasswordVisible(bool value)
        {
            using (ViewStateChangeDebounced())
            {
                ViewState.IsPasswordVisible = value;
            }
        }

        public Task<bool> UsernameCharacterIsValid(char value)
        {
            return Task.FromResult(value != '!');
        }

        public async Task LoginAsync()
        {
            //always validate state on submission
            ViewState.IsValid = EditContext.Validate();

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
            using (ViewStateChangeDebounced())
            {
                ViewState.SetDefaults();
                ResetValidationErrors();
            }
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
            if(e.IsIdle)
            {
                Reset();
            }
        }

        private void OnUserLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            using (ViewStateChangeDebounced())
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
                        }

                        ViewState.HasLoginError = true;
                        ViewState.LoginError = e.FailReason.ToString();
                        break;
                    //in all other cases make a full view state reset
                    default:
                        Reset();
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
            }
        }
    }
}
