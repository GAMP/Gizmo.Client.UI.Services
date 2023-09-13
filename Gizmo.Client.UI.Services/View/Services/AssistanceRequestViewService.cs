using Gizmo.Client.UI.View.States;
using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
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
            AssistanceRequestTypesViewStateLookupService assistanceRequestTypesViewStateLookupService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _localizationService = localizationService;
            _assistanceRequestTypesViewStateLookupService = assistanceRequestTypesViewStateLookupService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly ILocalizationService _localizationService;
        private readonly AssistanceRequestTypesViewStateLookupService _assistanceRequestTypesViewStateLookupService;
        #endregion

        #region FUNCTIONS

        public void SetSelectedAssistanceRequestType(int? assistanceRequestType)
        {
            ViewState.SelectedAssistanceRequestType = assistanceRequestType;
            ValidateProperty(() => ViewState.SelectedAssistanceRequestType);
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

                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Assistance request create error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
        }

        public async Task CancelAsync()
        {
            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                var result = await _gizmoClient.AssistanceRequestPendingCancelAsync();

                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Assistance request cancel error.");

                ViewState.HasError = true;
                ViewState.ErrorMessage = _localizationService.GetString("GIZ_GEN_AN_ERROR_HAS_OCCURED");
            }
            finally
            {
                ViewState.IsLoading = false;
                ViewState.RaiseChanged();
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

        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.LoginStateChange += OnLoginStateChange;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.LoginStateChange -= OnLoginStateChange;
            base.OnDisposing(isDisposing);
        }

        #endregion

        private async void OnLoginStateChange(object? sender, UserLoginStateChangeEventArgs e)
        {
            if (e.State == LoginState.LoggedIn)
            {
                try
                {
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
        }
    }
}
