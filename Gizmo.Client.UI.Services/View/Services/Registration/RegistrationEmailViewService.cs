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
    public sealed class RegistrationEmailViewService : ValidatingViewStateServiceBase<RegistrationEmailViewState>
    {
        #region CONSTRUCTOR
        public RegistrationEmailViewService(
            RegistrationEmailViewState viewState,
            ILogger<RegistrationEmailViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IRegistrationSessionService registrationSession,
            UserRegistrationViewState userRegistrationViewState,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _userRegistrationViewState = userRegistrationViewState;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly UserRegistrationViewState _userRegistrationViewState;
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

            var integrationPublicId = _userRegistrationViewState.SelectedProvider?.PublicId ?? Guid.Empty;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    Email = ViewState.Email,
                    DeliveryMethod = RegistrationDeliveryMethod.CodeDispatch,
                    IntegrationPublicId = integrationPublicId
                });

                switch (result.Result)
                {
                    case RegistrationStartCode.Success:
                        _registrationSession.SetStartResult(
                            result.Token ?? string.Empty,
                            result.Destination ?? string.Empty,
                            result.CodeLength,
                            RegistrationFlow.Email);
                        NavigationService.NavigateTo(ClientRoutes.RegistrationConfirmationRoute);
                        break;

                    case RegistrationStartCode.NonUniqueInput:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_REGISTRATION_VE_EMAIL_ADDRESS_USED));
                        break;

                    case RegistrationStartCode.NoRouteForDelivery:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_USER_CONFIRMATION_ERROR_PROVIDER_NO_ROUTE_FOR_DELIVERY));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) + $" {result.Result}";
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration email start error.");
                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
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
                    if (await _registrationService.ExistsAsync(ViewState.Email, cancellationToken))
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
