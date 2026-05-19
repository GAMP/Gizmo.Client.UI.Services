using Gizmo.Client;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationRedirectRoute)]
    public sealed class RegistrationRedirectViewService : ViewStateServiceBase<RegistrationRedirectViewState>
    {
        #region CONSTRUCTOR
        public RegistrationRedirectViewService(
            RegistrationRedirectViewState viewState,
            ILogger<RegistrationRedirectViewService> logger,
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

        public Task NavigateBackAsync()
        {
            _registrationSession.Clear();
            NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
            return Task.CompletedTask;
        }

        public void Reset()
        {
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.RaiseChanged();
        }

        #endregion

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ViewState.RedirectUrl = null;
            ViewState.HasError = false;
            ViewState.ErrorMessage = string.Empty;
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            var integrationPublicId = _userRegistrationViewState.SelectedProvider?.PublicId ?? Guid.Empty;

            try
            {
                var result = await _registrationService.StartAsync(new RegistrationStartRequest
                {
                    DeliveryMethod = RegistrationDeliveryMethod.Redirect,
                    IntegrationPublicId = integrationPublicId
                }, cancellationToken);

                switch (result.Result)
                {
                    case RegistrationStartCode.Success when result.RedirectUrl is not null:
                        _registrationSession.SetStartResult(
                            result.Token ?? string.Empty,
                            result.Destination ?? string.Empty,
                            result.CodeLength,
                            RegistrationFlow.None);
                        ViewState.RedirectUrl = result.RedirectUrl;
                        break;

                    case RegistrationStartCode.Success:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED));
                        break;

                    default:
                        ViewState.HasError = true;
                        ViewState.ErrorMessage = _localizationService.GetString(nameof(Gizmo.Client.UI.Resources.Properties.Resources.GIZ_GEN_AN_ERROR_HAS_OCCURRED)) + $" {result.Result}";
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Redirect registration start error.");
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
    }
}
