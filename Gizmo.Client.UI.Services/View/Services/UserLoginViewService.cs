using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Route(ClientRoutes.LoginRoute)]
    [Register()]
    public sealed class UserLoginViewService : ValidatingViewStateServiceBase<UserLoginViewState>
    {
        public UserLoginViewService(
            UserLoginViewState viewState,
            ILogger<UserLoginViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
        }

        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;        

        public void SetLoginMethod(UserLoginType userLoginType)
        {
            if (ViewState.LoginType != userLoginType)
            {
                ViewState.LoginType = userLoginType;
                SetLoginName(string.Empty);
                DebounceViewStateChanged();
            }
        }

        public void SetLoginName(string value)
        {
            ViewState.LoginName = value;
            ValidateProperty(() => ViewState.LoginName);
        }

        public void SetPassword(string value)
        {
            ViewState.Password = value;
            ValidateProperty(() => ViewState.Password);
        }

        public void SetPasswordVisible(bool value)
        {
            ViewState.IsPasswordVisible = value;
            DebounceViewStateChanged();
        }

        public Task<bool> UsernameCharacterIsValid(char value)
        {
            return Task.FromResult(true);
        }

        public async Task LoginAsync()
        {
            //always validate state on submission
            Validate();

            //model validation is pending, we cant proceed
            if (ViewState.IsValid != true)
                return;

            string? loginName = ViewState.LoginName;
            string? password = ViewState.Password;

            if (string.IsNullOrEmpty(loginName))
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

        public void Reset()
        {
            ViewState.SetDefaults();
            ResetValidationState();
        }

        protected override Task OnNavigatedOut(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            //whenever we move away from login page we should make full view state reset
            Reset();
            return base.OnNavigatedOut(navigationParameters, cancellationToken);
        }

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            return base.OnNavigatedIn(navigationParameters, cancellationToken);
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
                        case Web.Api.Models.LoginResult.InvalidPassword:
                            ViewState.Password = null;
                            break;
                        //clear both username and password in any other error case
                        default:
                            ViewState.LoginName = null;
                            ViewState.Password = null;
                            break;
                    }

                    //process login error reason
                    ViewState.HasLoginError = true;

                    string ERROR_MESSAGE = string.Empty;

                    switch (e.FailReason)
                    {
                        case Web.Api.Models.LoginResult.AccountDisabled:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_ACCOUNT_DISABLED");
                            break;
                        case Web.Api.Models.LoginResult.AlreadyLoggedIn:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_ALREADY_LOGGED_IN");
                            break;
                        case Web.Api.Models.LoginResult.Denied:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_DENIED");
                            break;
                        case Web.Api.Models.LoginResult.Failed:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_FAILED");
                            break;
                        case Web.Api.Models.LoginResult.InsufficientBalance:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_INSUFFICIENT_BALANCE");
                            break;
                        case Web.Api.Models.LoginResult.InvalidCredentials:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_INVALID_CREDENTIALS");
                            break;
                        case Web.Api.Models.LoginResult.InvalidParameters:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_INVALID_PARAMETERS");
                            break;
                        case Web.Api.Models.LoginResult.InvalidPassword:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_INVALID_PASSWORD");
                            break;
                        case Web.Api.Models.LoginResult.InvalidUserName:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_INVALID_USERNAME");
                            break;
                        case Web.Api.Models.LoginResult.MaximumSessionsReached:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_MAX_SESSIONS_REACHED");
                            break;
                        case Web.Api.Models.LoginResult.NotInWaitingLine:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_NOT_IN_WAITING_LINE");
                            break;
                        case Web.Api.Models.LoginResult.Success:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_SUCESS");
                            break;
                        case Web.Api.Models.LoginResult.RestrictedByAge:
                            ERROR_MESSAGE = _localizationService.GetString("GIZ_LOGIN_RESULT_RESTRICTED_BY_AGE");
                            break;
                        default:
                            ERROR_MESSAGE = e.FailReason.ToString();
                            break;
                    }

                    ViewState.LoginError = ERROR_MESSAGE;

                    break;
                case LoginState.LoginCompleted:
                    Reset();
                    break;
                default:
                    break;
            }

            switch (e.State)
            {
                //LoggingIn state is the only state
                case LoginState.LoggingIn:
                    ViewState.IsLogginIn = true;
                    break;
                case LoginState.LoggingOut:
                    ViewState.IsLogginOut = true;
                    break;
                case LoginState.LoggedOut:
                case LoginState.LoginCompleted:
                case LoginState.LoginFailed:
                    ViewState.IsLogginIn = false;
                    ViewState.IsLogginOut = false;
                    break;
                default:
                    break;
            }

            DebounceViewStateChanged();
        }      
    }
}
