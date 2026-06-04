using Gizmo.Client.UI.Services;
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
            IAuthenticationService authenticationService,
            IAuthenticationSessionService authenticationSession,
            ILocalizationService localizationService,
            IPasswordRecoveryService passwordRecoveryService,
            IPasswordRecoverySessionService passwordRecoverySession) : base(viewState, logger, serviceProvider)
        {
            _authenticationService = authenticationService;
            _authenticationSession = authenticationSession;
            _localizationService = localizationService;
            _passwordRecoveryService = passwordRecoveryService;
            _passwordRecoverySession = passwordRecoverySession;
        }

        private readonly IAuthenticationService _authenticationService;
        private readonly IAuthenticationSessionService _authenticationSession;
        private readonly ILocalizationService _localizationService;
        private readonly IPasswordRecoveryService _passwordRecoveryService;
        private readonly IPasswordRecoverySessionService _passwordRecoverySession;

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

        public void SetPin(string value)
        {
            ViewState.Pin = value;
            ValidateProperty(() => ViewState.Pin);
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
            string? pin = ViewState.Pin;

            if (string.IsNullOrEmpty(loginName))
                return;

            if (ViewState.IsLogginIn)
                return;

            ViewState.HasLoginError = false;
            ViewState.LoginError = null;
            ViewState.IsLogginIn = true;
            _authenticationSession.SetLoggingIn();
            DebounceViewStateChanged();

            try
            {
                // TODO: Add a real phone-login flow when the server exposes a contract that verifies
                // a phone code and issues an auth token. Until then MobilePhone intentionally sends
                // LoginName as Username through the password accesstoken endpoint, preserving the
                // existing phone-as-username behavior; phone verification endpoints do not issue
                // login tokens.
                var result = await _authenticationService.LoginAsync(new AuthenticationLoginRequest
                {
                    Username = loginName,
                    Password = password,
                    Pin = pin,
                });

                Logger.LogTrace("Client login result {success} {error}", result.Success, result.Error);

                if (result.Success)
                {
                    _authenticationSession.SetLoggedIn(loginName);
                    Reset();
                }
                else
                {
                    ApplyLoginError(result.Error);
                    _authenticationSession.SetLoginFailed(result.Error, result.Message);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "User initiated client login error.");
                ApplyLoginError(AuthenticationLoginError.Unknown);
                _authenticationSession.SetLoginFailed(AuthenticationLoginError.Unknown, ex.Message);
            }
            finally
            {
                ViewState.IsLogginIn = false;
                DebounceViewStateChanged();
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

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            await base.OnNavigatedIn(navigationParameters, cancellationToken);
            await LoadRecoveryAvailabilityAsync(cancellationToken);
        }

        private async Task LoadRecoveryAvailabilityAsync(CancellationToken ct)
        {
            try
            {
                var providers = await _passwordRecoveryService.GetProvidersAsync(ct);
                if (providers.Count == 1)
                {
                    ViewState.IsPasswordRecoveryAvailable = true;
                    _passwordRecoverySession.SetActiveProvider(providers[0]);
                }
                else
                {
                    ViewState.IsPasswordRecoveryAvailable = false;
                    if (providers.Count > 1)
                        Logger.LogWarning("Password recovery provider contract violation: {Count} supported providers returned; expected at most 1.", providers.Count);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load password recovery providers.");
                ViewState.IsPasswordRecoveryAvailable = false;
            }
            finally
            {
                ViewState.RaiseChanged();
            }
        }

        private void ApplyLoginError(AuthenticationLoginError error)
        {
            ViewState.HasLoginError = true;

            switch (error)
            {
                case AuthenticationLoginError.InvalidCredentials:
                    ViewState.LoginError = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_LOGIN_RESULT_INVALID_CREDENTIALS));
                    ViewState.Password = null;
                    break;
                case AuthenticationLoginError.Network:
                    ViewState.LoginError = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                    ViewState.LoginName = null;
                    ViewState.Password = null;
                    ViewState.Pin = null;
                    break;
                case AuthenticationLoginError.ServerError:
                case AuthenticationLoginError.Unknown:
                default:
                    ViewState.LoginError = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_LOGIN_RESULT_FAILED));
                    ViewState.LoginName = null;
                    ViewState.Password = null;
                    ViewState.Pin = null;
                    break;
            }
        }
    }
}
