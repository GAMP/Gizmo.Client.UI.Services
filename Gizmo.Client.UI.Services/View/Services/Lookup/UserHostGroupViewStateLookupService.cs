using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class UserHostGroupViewStateLookupService : ViewStateLookupServiceBase<int, UserHostGroupViewState>
    {
        private readonly IGizmoClient _gizmoClient;
        public UserHostGroupViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<UserHostGroupViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.HostGroupChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.HostGroupChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, UserHostGroupViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserHostGroupsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<UserHostGroupViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserHostGroupGetAsync(lookUpkey, cToken);

           return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<UserHostGroupViewState> UpdateViewStateAsync(UserHostGroupViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserHostGroupGetAsync(viewState.Id, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override UserHostGroupViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<UserHostGroupViewState>();

            defaultState.Id = lookUpkey;

            defaultState.Name = "Default name";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private UserHostGroupViewState Map (UserHostGroupModel model, UserHostGroupViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.Name = model.Name;
            
            return result;
        }

        #endregion
    }
}
