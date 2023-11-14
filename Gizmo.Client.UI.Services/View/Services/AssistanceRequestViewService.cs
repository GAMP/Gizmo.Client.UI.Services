﻿using System.Threading;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.UI.View.States;
using Gizmo.Web.Api.Messaging;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class AssistanceRequestViewService : ValidatingViewStateServiceBase<AssistanceRequestViewState>
    {
        #region CONSTRUCTOR
        public AssistanceRequestViewService(AssistanceRequestViewState viewState,
            ILogger<AssistanceRequestViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            AssistanceRequestTypesViewStateLookupService assistanceRequestTypesViewStateLookupService,
            IClientNotificationService notificationService,
            IClientDialogService dialogService,
            IOptions<AssistanceRequestOptions> assistanceRequestOptions) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _assistanceRequestTypesViewStateLookupService = assistanceRequestTypesViewStateLookupService;
            _notificationService = notificationService;
            _dialogService = dialogService;
            _assistanceRequestOptions = assistanceRequestOptions;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly AssistanceRequestTypesViewStateLookupService _assistanceRequestTypesViewStateLookupService;
        private readonly IClientNotificationService _notificationService;
        private readonly IClientDialogService _dialogService;
        private readonly IOptions<AssistanceRequestOptions> _assistanceRequestOptions;
        #endregion

        #region FUNCTIONS

        public void SetSelectedAssistanceRequestType(int? assistanceRequestType)
        {
            ViewState.SelectedAssistanceRequestType = assistanceRequestType;
            ValidateProperty(() => ViewState.SelectedAssistanceRequestType);
        }

        public void SetNote(string value)
        {
            ViewState.Note = value;
        }

        public async Task SubmitAsync()
        {
            Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.AssistanceRequestCreateAsync(new AssistanceRequestModelUserCreate()
                {
                    AssistanceRequestTypeId = ViewState.SelectedAssistanceRequestType.Value,
                    Note = ViewState.Note
                });

                ViewState.SelectedAssistanceRequestType = null;
                ViewState.Note = null;

                ViewState.AnyPending = await _gizmoClient.AssistanceRequestAnyPendingGetAsync();

                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                var userMenuViewService = ServiceProvider.GetRequiredService<UserMenuViewService>();
                userMenuViewService.CloseAssistanceRequests();

                var dialogResult = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_ASSISTANCE_REQUEST_SENT_TITLE"), _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_SENT_MESSAGE"), AlertDialogButtons.OK, AlertTypes.Success);
                _ = await dialogResult.WaitForResultAsync();
            }
            catch (Exception ex)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                Logger.LogError(ex, "Assistance request create error.");

                await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_FAILED_TO_PROCESS"));
            }
        }

        public async Task CancelAsync()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.AssistanceRequestPendingCancelAsync();
                ViewState.AnyPending = await _gizmoClient.AssistanceRequestAnyPendingGetAsync();

                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();

                Logger.LogError(ex, "Assistance request cancel error.");

                await _notificationService.ShowAlertNotification(AlertTypes.Danger, _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED"), _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_FAILED_TO_PROCESS"));
            }
        }

        public void Clear()
        {
            ViewState.SelectedAssistanceRequestType = null;
            ViewState.Note = null;
            ViewState.RaiseChanged();
        }

        #endregion

        #region OVERRIDES

        protected override void OnValidate(FieldIdentifier fieldIdentifier, ValidationTrigger validationTrigger)
        {
            if (fieldIdentifier.FieldEquals(() => ViewState.SelectedAssistanceRequestType))
            {
                if (!ViewState.SelectedAssistanceRequestType.HasValue)
                {
                    AddError(() => ViewState.SelectedAssistanceRequestType, _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_VE_TYPE_IS_REQUIRED"));
                }
            }
        }

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            _gizmoClient.OnAPIEventMessage += OnAPIEventMessage;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.OnAPIEventMessage -= OnAPIEventMessage;
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        #endregion

        private async void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is AssistanceRequestStatusChangeEventMessage assistanceRequestStatusChangeEventMessage)
            {
                //TODO: AAA Don't we need to verify we have the same id?
                if (assistanceRequestStatusChangeEventMessage.Status != AssistanceRequestStatus.Pending)
                {
                    try
                    {
                        if (assistanceRequestStatusChangeEventMessage.Status == AssistanceRequestStatus.Accepted)
                        {
                            _ = await _notificationService.ShowAlertNotification(Gizmo.UI.AlertTypes.Info, _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_RESPONSE_TITLE"), _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_RESPONSE_ACCEPTED"));
                        }
                        else if (assistanceRequestStatusChangeEventMessage.Status == AssistanceRequestStatus.Rejected)
                        {
                            _ = await _notificationService.ShowAlertNotification(Gizmo.UI.AlertTypes.Warning, _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_RESPONSE_TITLE"), _localizationService.GetString("GIZ_ASSISTANCE_REQUEST_RESPONSE_REJECTED"));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Failed to inform user about assistance request status.");
                    }

                    ViewState.AnyPending = false;

                    DebounceViewStateChanged();
                }
            }
        }

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedIn)
            {
                try
                {
                    ViewState.IsEnabled = false;
                    ViewState.SelectedAssistanceRequestType = null;
                    ViewState.Note = null;

                    ViewState.IsInitializing = true;
                    ViewState.IsInitialized = null;
                    DebounceViewStateChanged();

                    if (!_assistanceRequestOptions.Value.Disabled)
                    {
                        var states = await _assistanceRequestTypesViewStateLookupService.GetStatesAsync();
                        ViewState.AssistanceRequestTypes = states.Where(a => !a.IsDeleted);
                        ViewState.AnyPending = await _gizmoClient.AssistanceRequestAnyPendingGetAsync();

                        ViewState.IsEnabled = true;
                    }

                    ViewState.IsInitialized = true;
                }
                catch (Exception ex)
                {
                    ViewState.IsInitialized = false;

                    Logger.LogError(ex, "Failed to obtain user assistance requests.");
                }
                finally
                {
                    ViewState.IsInitializing = false;
                    DebounceViewStateChanged();
                }
            }
            else if (e.State == LoginState.LoggedOut)
            {
                Clear();
            }
        }
    }
}
