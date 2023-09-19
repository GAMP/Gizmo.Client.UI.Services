using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Messaging;
using Gizmo.Web.Api.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class AssistanceRequestViewService : ValidatingViewStateServiceBase<AssistanceRequesetViewState>
    {
        #region CONSTRUCTOR
        public AssistanceRequestViewService(AssistanceRequesetViewState viewState,
            ILogger<AssistanceRequestViewService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            ILocalizationService localizationService,
            AssistanceRequestTypesViewStateLookupService assistanceRequestTypesViewStateLookupService,
            IClientNotificationService notificationService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _assistanceRequestTypesViewStateLookupService = assistanceRequestTypesViewStateLookupService;
            _notificationService = notificationService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly AssistanceRequestTypesViewStateLookupService _assistanceRequestTypesViewStateLookupService;
        private readonly IClientNotificationService _notificationService;
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

                ViewState.AnyPending = await _gizmoClient.AssistanceRequestAnyPendingGetAsync();

                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
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
        
        private void OnAPIEventMessage(object? sender, IAPIEventMessage e)
        {
            if (e is AssistanceRequestStatusChangeEventMessage assistanceRequestStatusChangeEventMessage)
            {
                //TODO: AAA Don't we need to verify we have the same id?
                if (assistanceRequestStatusChangeEventMessage.Status != AssistanceRequestStatus.Pending)
                {
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
                    ViewState.SelectedAssistanceRequestType = null;
                    ViewState.Note = null;

                    ViewState.IsInitializing = true;
                    ViewState.IsInitialized = null;
                    DebounceViewStateChanged();

                    ViewState.AssistanceRequestTypes = await _assistanceRequestTypesViewStateLookupService.GetStatesAsync();

                    ViewState.AnyPending = await _gizmoClient.AssistanceRequestAnyPendingGetAsync();

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
