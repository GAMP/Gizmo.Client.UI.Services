using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppExeViewStateLookupService : ViewStateLookupServiceBase<int, AppExeViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public AppExeViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppExeViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.AppExeChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.AppExeChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, AppExeViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserExecutablesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppExeViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserExecutableGetAsync(lookUpkey, cToken);

            return clientResult is null  ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppExeViewState> UpdateViewStateAsync(AppExeViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserExecutableGetAsync(viewState.ExecutableId, cToken);
            
            return clientResult is null  ? CreateDefaultViewState(viewState.ExecutableId) : Map(clientResult, viewState);
        }
        protected override AppExeViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppExeViewState>();

            defaultState.ExecutableId = lookUpkey;

            defaultState.Caption = "Default name";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AppExeViewState Map(UserExecutableModel model, AppExeViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.ApplicationId = model.ApplicationId;
            result.Caption = model.Caption;
            result.Description = model.Description;
            result.DisplayOrder = model.DisplayOrder;
            result.PersonalFiles = model.PersonalFiles.Select(a => new AppExePersonalFileViewState()
            {
                PersonalFileId = a.PersonalFileId
            });
            result.ImageId = model.ImageId;
            result.Options = model.Options;
            result.Modes = model.Modes;

            result.AutoLaunch = model.Options.HasFlag(ExecutableOptionType.AutoLaunch);

            return result;
        }
        #endregion
    }
}
