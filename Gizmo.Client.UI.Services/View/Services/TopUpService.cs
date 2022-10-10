using Gizmo.Client.UI.View.States;
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
            IServiceProvider serviceProvider, IGizmoClient gizmoClient) : base(viewState, logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }
        #endregion

        #region FIELDS
        private readonly IGizmoClient _gizmoClient;
        #endregion

        #region PROPERTIES

        #endregion

        #region FUNCTIONS

        public void SelectPreset(decimal amount)
        {
            if (ViewState.Presets.Contains(amount))
            {
                ViewState.Amount = amount;
                ViewState.RaiseChanged();
            }
        }

        public Task SubmitAsync()
        {
            ViewState.IsValid = EditContext.Validate();

            if (ViewState.IsValid != true)
                return Task.CompletedTask;

            ViewState.PageIndex = 1;
            ViewState.RaiseChanged();

            return Task.CompletedTask;
        }

        public Task PayFromPC()
        {
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