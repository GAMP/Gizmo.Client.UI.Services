using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Gizmo.Web.Api.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register]
    public sealed class PersonalFileViewStateLookupService : ViewStateLookupServiceBase<int, PersonalFileViewState>
    {
        private readonly IGizmoClient _gizmoClient;

        public PersonalFileViewStateLookupService(
            IGizmoClient gizmoClient,
            ILogger<PersonalFileViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        #region OVERRIDED FUNCTIONS
        protected override async Task<IDictionary<int, PersonalFileViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserPersonalFilesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<PersonalFileViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPersonalFileGetAsync(lookUpkey, cToken);

            return clientResult is null ? CreateDefaultViewState(lookUpkey) : Map(clientResult);
        }
        protected override async ValueTask<PersonalFileViewState> UpdateViewStateAsync(PersonalFileViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPersonalFileGetAsync(viewState.PersonalFileId, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override PersonalFileViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<PersonalFileViewState>();

            defaultState.PersonalFileId = lookUpkey;

            defaultState.Caption = "Default caption";

            return defaultState;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        private PersonalFileViewState Map(UserPersonalFileModel model, PersonalFileViewState? viewState = null)
        {
            var result = viewState ?? CreateDefaultViewState(model.Id);
            
            result.Caption = model.Caption;
            
            return result;
        }
        #endregion
    }
}
