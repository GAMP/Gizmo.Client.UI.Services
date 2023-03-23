using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

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

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var personalFiles = await _gizmoClient.UserPersonalFilesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in personalFiles.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.PersonalFileId = item.Id;
                viewState.Caption = item.Caption;

                AddOrUpdateViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<PersonalFileViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserPersonalFileGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.PersonalFileId = item.Id;
            viewState.Caption = item.Caption;

            return viewState;
        }
        protected override PersonalFileViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<PersonalFileViewState>();

            defaultState.PersonalFileId = lookUpkey;

            defaultState.Caption = "Default caption";

            return defaultState;
        }
    }
}
