using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class TopUpService : ValidatingViewStateServiceBase<TopUpViewState>
    {
        #region CONSTRUCTOR
        public TopUpService(TopUpViewState viewState,
            ILogger<TopUpService> logger,
            IServiceProvider serviceProvider,
            IGizmoClient gizmoClient,
            IClientDialogService dialogService) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
            _dialogService = dialogService;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        private readonly IClientDialogService _dialogService;
        private CancellationTokenSource _dialogCancellationTokenSource;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public async Task ShowDialogAsync()
        {
            if (_dialogCancellationTokenSource != null)
            {
                _dialogCancellationTokenSource.Dispose();
            }

            _dialogCancellationTokenSource = new CancellationTokenSource();

            var s = await _dialogService.ShowTopUpDialogAsync(_dialogCancellationTokenSource.Token);
            if (s.Result == DialogAddResult.Success)
            {
                try
                {
                    var result = await s.WaitForDialogResultAsync();
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        public void SelectPreset(decimal amount)
        {
            if (ViewState.Presets.Contains(amount))
            {
                ViewState.Amount = amount;
                ViewState.RaiseChanged();
            }
        }

        public async Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return;

            ViewState.IsLoading = true;
            ViewState.RaiseChanged();

            try
            {
                // Simulate task.
                await Task.Delay(2000);

                ViewState.IsLoading = false;

                ViewState.PageIndex = 1;
                ViewState.RaiseChanged();
            }
            catch
            {

            }
            finally
            {

            }
        }

        public Task PayFromPC()
        {
            _dialogCancellationTokenSource.Cancel();

            ViewState.PageIndex = 0;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        #endregion

        protected override async Task OnInitializing(CancellationToken ct)
        {
            await base.OnInitializing(ct);

            ViewState.Presets.Add(5);
            ViewState.Presets.Add(10);
            ViewState.Presets.Add(15);
            ViewState.Presets.Add(20);
            ViewState.Presets.Add(25);
            ViewState.Presets.Add(30);
            ViewState.Presets.Add(35);
        }
    }
}