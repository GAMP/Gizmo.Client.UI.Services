using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    [Route(ClientRoutes.RegistrationIndexRoute)]
    public sealed class UserRegistrationIndexViewService : ViewStateServiceBase<UserRegistrationIndexViewState>
    {
        #region CONSTRUCTOR
        public UserRegistrationIndexViewService(UserRegistrationIndexViewState viewState,
            ILogger<UserRegistrationIndexViewService> logger,
            IServiceProvider serviceProvider,
            IUserRegistrationService registrationService,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _registrationService = registrationService;
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IUserRegistrationService _registrationService;
        private readonly IClientDialogService _dialogService;
        #endregion

        public async Task<bool> ProcessUserAgreements(CancellationToken cancellationToken = default)
        {
            var userAgreements = await _registrationService.GetAgreementsAsync(cancellationToken);

            var userAgreementStates = userAgreements.Select(a => new UserAgreementViewState()
            {
                Id = a.Id,
                Name = a.Name,
                Agreement = a.Agreement,
                IsRejectable = a.IsRejectable,
                IgnoreState = a.IgnoreState,
                AcceptState = UserAgreementAcceptState.None
            }).ToList();

            foreach (var userAgreement in userAgreementStates)
            {
                var addDialogResult = await _dialogService.ShowUserAgreementDialogAsync(new UserAgreementDialogParameters()
                {
                    Name = userAgreement.Name,
                    Agreement = userAgreement.Agreement,
                    IsRejectable = userAgreement.IsRejectable
                }, cancellationToken);

                if (addDialogResult.Result == AddComponentResultCode.Opened)
                {
                    var dialogResult = await addDialogResult.WaitForResultAsync(cancellationToken);
                    if (addDialogResult.Result == AddComponentResultCode.Ok)
                    {
                        if (dialogResult!.Accepted)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Accepted;
                        }
                        else
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Rejected;
                        }
                    }
                    else if (addDialogResult.Result == AddComponentResultCode.Dismissed)
                    {
                        if (userAgreement.IsRejectable)
                        {
                            userAgreement.AcceptState = UserAgreementAcceptState.Rejected;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (addDialogResult.Result == AddComponentResultCode.Canceled)
                    {
                        //dialog was canceled, this will only happen if function caller have cancelled
                        //or a global cancellation happened
                        return false;
                    }
                }
            }

            ViewState.UserAgreementStates = userAgreementStates;

            return true;
        }

        public void ClearAll()
        {
            var userRegistrationConfirmationService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationViewService>();
            var userRegistrationConfirmationMethodService = ServiceProvider.GetRequiredService<UserRegistrationConfirmationMethodViewService>();
            var userRegistrationBasicFieldsService = ServiceProvider.GetRequiredService<UserRegistrationBasicFieldsViewService>();
            var userRegistrationAdditionalFieldsService = ServiceProvider.GetRequiredService<UserRegistrationAdditionalFieldsViewService>();
            var userRegistrationService = ServiceProvider.GetRequiredService<UserRegistrationViewService>();

            userRegistrationConfirmationService.Clear();
            userRegistrationConfirmationMethodService.Clear();
            userRegistrationBasicFieldsService.Clear();
            userRegistrationAdditionalFieldsService.Clear();
            userRegistrationService.ClearProvider();

            ViewState.UserAgreementStates = Enumerable.Empty<UserAgreementViewState>();
            DebounceViewStateChanged();
        }

        #region OVERRIDES

        protected override async Task OnNavigatedIn(NavigationParameters navigationParameters, CancellationToken cancellationToken = default)
        {
            ClearAll();

            try
            {
                var providers = await _registrationService.GetProvidersAsync(cancellationToken);

                var agreementStatus = await ProcessUserAgreements(cancellationToken);

                if (agreementStatus)
                {
                    var userRegistrationService = ServiceProvider.GetRequiredService<UserRegistrationViewService>();

                    var userGroupDefaultRequiredInfo = await _registrationService.GetRequiredUserInfoAsync(cancellationToken);

                    userRegistrationService.SetUserGroupDefaultRequiredInfo(userGroupDefaultRequiredInfo);

                    if (providers.Count > 0)
                    {
                        NavigationService.NavigateTo(ClientRoutes.RegistrationProvidersRoute);
                    }
                    else
                    {
                        NavigationService.NavigateTo(ClientRoutes.RegistrationBasicFieldsRoute);
                    }
                }
                else
                {
                    NavigationService.NavigateTo(ClientRoutes.LoginRoute);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Registration service error.");
            }
        }

        #endregion
    }
}
