using System;
using Gizmo.Client.UI.Services;
using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class AppViewStateLookupService : ViewStateLookupServiceBase<int, AppViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public AppViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<AppViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.AppChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.AppChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, AppViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserApplicationsGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<AppViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<AppViewState> UpdateViewStateAsync(AppViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserApplicationGetAsync(viewState.ApplicationId, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override AppViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppViewState>();

            defaultState.ApplicationId = lookUpkey;

            defaultState.Title = "Default title";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private AppViewState Map(UserApplicationModel model, AppViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);

            result.Title = model.Title;
            result.Description = model.Description;
            result.ApplicationCategoryId = model.ApplicationCategoryId;
            result.ReleaseDate = model.ReleaseDate;
            result.AddDate = model.AddDate;
            result.DeveloperId = model.DeveloperId;
            result.PublisherId = model.PublisherId;
            result.ImageId = model.ImageId;

            return result;
        }
        #endregion

        /// <summary>
        /// Gets filtered app states.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>App states.</returns>
        /// <remarks>
        /// Only app states that pass current app profile will be returned.
        /// </remarks>
        public async Task<IEnumerable<AppViewState>> GetFilteredStatesAsync(CancellationToken cancellationToken = default)
        {
            var states = await GetStatesAsync(cancellationToken);

            var appExeViewStateLookupService = ServiceProvider.GetRequiredService<AppExeViewStateLookupService>();

            var appExeViewStates = await appExeViewStateLookupService.GetFilteredStatesAsync(cancellationToken);

            return states.Where(state => _gizmoClient.AppCurrentProfilePass(state.ApplicationId))
                         .Where(state => appExeViewStates.Where(a => a.ApplicationId == state.ApplicationId).Count() > 0)
                         .ToList();
        }
    }
}
