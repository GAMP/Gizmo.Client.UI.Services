using Gizmo.Client.UI.Services;
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
        protected override Task OnInitializing(CancellationToken ct)
        {
            _gizmoClient.PersonalFileChange += async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            return base.OnInitializing(ct);
        }
        protected override void OnDisposing(bool isDisposing)
        {
            _gizmoClient.PersonalFileChange -= async (e, v) => await HandleChangesAsync(v.EntityId, v.ModificationType.FromModificationType());
            base.OnDisposing(isDisposing);
        }
        protected override async Task<IDictionary<int, PersonalFileViewState>> DataInitializeAsync(CancellationToken cToken)
        {
            var clientResult = await _gizmoClient.UserPersonalFilesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            return clientResult.Data.ToDictionary(key => key.Id, value => Map(value));
        }
        protected override async ValueTask<PersonalFileViewState> CreateViewStateAsync(int key, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPersonalFileGetAsync(key, cToken);

            return clientResult is null ? CreateDefaultViewState(key) : Map(clientResult);
        }
        protected override async ValueTask<PersonalFileViewState> UpdateViewStateAsync(int key, PersonalFileViewState viewState, CancellationToken cToken = default)
        {
            var clientResult = await _gizmoClient.UserPersonalFileGetAsync(viewState.PersonalFileId, cToken);

            return clientResult is null ? viewState : Map(clientResult, viewState);
        }
        protected override PersonalFileViewState CreateDefaultViewState(int key)
        {
            var defaultState = ServiceProvider.GetRequiredService<PersonalFileViewState>();

            defaultState.PersonalFileId = key;

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
