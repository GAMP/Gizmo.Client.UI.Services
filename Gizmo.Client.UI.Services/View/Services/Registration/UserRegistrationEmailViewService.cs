using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationEmailRoute)]
    public sealed class UserRegistrationEmailViewService : ValidatingViewStateServiceBase<UserRegistrationEmailViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationEmailViewService(
            UserRegistrationEmailViewState viewState,
            ILogger<UserRegistrationEmailViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region FUNCTIONS

        public void SetEmail(string value)
        {
            ViewState.Email = value;
            ValidateProperty(() => ViewState.Email);
        }

        public async Task SubmitAsync()
        {
            ViewState.IsLoading = true;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();

            Validate();

            if (ViewState.IsValid != true)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
                return;
            }

            var methodId = _registrationSession.SelectedProvider?.MethodId ?? 0;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    MethodId = methodId,
                    Kind = RegistrationStartKind.Email,
                    Value = ViewState.Email
                });

                switch (result)
                {
                    case RegistrationStartResult.CodeInputRequired r:
                        _registrationSession.SetStartResult(
                            r.Token,
                            r.Destination ?? string.Empty,
                            r.CodeLength,
                            r.ExpiresInSeconds,
                            RegistrationFlow.Email);
                        _registrationSession.SetContactDetails(ViewState.Email);
                        NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);
                        break;

                    case RegistrationStartResult.Failed { Code: RegistrationStartCode.NonUniqueInput }:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED));
                        break;

                    case RegistrationStartResult.Failed { Code: RegistrationStartCode.InvalidInput }:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_EMAIL_INVALID));
                        break;

                    case RegistrationStartResult.Failed f:
                        Logger.LogWarning("Registration email start failed for method {MethodId}: {Result}", methodId, f.Code);
                        NavigateToProvidersWithProviderFailure();
                        break;

                    default:
                        Logger.LogWarning("Registration email start returned an unexpected result shape for method {MethodId}.", methodId);
                        NavigateToProvidersWithProviderFailure();
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration email start error.");
                NavigateToProvidersWithProviderFailure();
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        private void NavigateToProvidersWithProviderFailure()
        {
            _registrationSession.SetFailedProviderChannelGuid(_registrationSession.SelectedProvider?.ChannelGuid ?? Guid.Empty);
            NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
        }

        #endregion

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
        }

        #region OVERRIDES

        protected override Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.Email = null;
            ViewState.IsLoading = false;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ResetValidationState();
            ViewState.RaiseChanged();
            return Task.CompletedTask;
        }

        protected override async Task<IEnumerable<string>> OnValidateAsync(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger, CancellationToken cancellationToken = default)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.Email) && !string.IsNullOrEmpty(ViewState.Email))
            {
                try
                {
                    if (await _registrationService.EmailExistAsync(ViewState.Email, cancellationToken))
                    {
                        return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED)) };
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Cannot validate email.");
                    return new string[] { _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_CANNOT_VALIDATE_EMAIL)) };
                }
            }

            return await base.OnValidateAsync(fieldIdentifier, validationTrigger, cancellationToken);
        }

        protected override AsyncValidatedDetermineResult OnDetermineIsAsyncPropertiesValidated()
        {
            if (IsAsyncValidated(() => ViewState.Email))
                return AsyncValidatedDetermineResult.DefaultTrue;

            return base.OnDetermineIsAsyncPropertiesValidated();
        }

        #endregion
    }
}
