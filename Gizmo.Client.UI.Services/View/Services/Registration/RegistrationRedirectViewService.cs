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
            RegistrationProvidersViewService registrationProvidersViewService,
            ILocalizationService localizationService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _registrationSession = registrationSession;
            _userRegistrationViewState = userRegistrationViewState;
            _registrationProvidersViewService = registrationProvidersViewService;
            _localizationService = localizationService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IRegistrationSessionService _registrationSession;
        private readonly UserRegistrationViewState _userRegistrationViewState;
        private readonly RegistrationProvidersViewService _registrationProvidersViewService;
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

            var provider = _userRegistrationViewState.SelectedProvider;
            var integrationPublicId = provider?.PublicId ?? Guid.Empty;
            var channelGuid = provider?.ChannelGuid ?? Guid.Empty;

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
                            result.ExpiresInSeconds,
                            RegistrationFlow.None);
                        ViewState.RedirectUrl = result.RedirectUrl;
                        break;

                    default:
                        Logger.LogWarning("Redirect registration start failed: {Result}", result.Result);
                        _registrationProvidersViewService.SetProviderError(channelGuid);
                        NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Redirect registration start error.");
                _registrationProvidersViewService.SetProviderError(channelGuid);
                NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
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
