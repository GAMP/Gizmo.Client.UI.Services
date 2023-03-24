using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;

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

        protected override async Task<bool> DataInitializeAsync(CancellationToken cToken)
        {
            var executables = await _gizmoClient.UserExecutablesGetAsync(new() { Pagination = new() { Limit = -1 } }, cToken);

            foreach (var item in executables.Data)
            {
                var viewState = CreateDefaultViewState(item.Id);

                viewState.ExecutableId = item.Id;
                viewState.ApplicationId = item.ApplicationId;
                viewState.Caption = item.Caption;
                viewState.Description = item.Description;
                viewState.DisplayOrder = item.DisplayOrder;
                viewState.Options = item.Options;
                viewState.PersonalFiles = item.PersonalFiles.Select(a => new AppExePersonalFileViewState()
                {
                    PersonalFileId = a.PersonalFileId
                });
                viewState.ImageId = item.ImageId;

                AddOrUpdateViewState(item.Id, viewState);
            }

            return true;
        }
        protected override async ValueTask<AppExeViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cToken = default)
        {
            var item = await _gizmoClient.UserExecutableGetAsync(lookUpkey, cToken);

            var viewState = CreateDefaultViewState(lookUpkey);

            if (item is null)
                return viewState;

            viewState.ExecutableId = item.Id;
            viewState.ApplicationId = item.ApplicationId;
            viewState.Caption = item.Caption;
            viewState.Description = item.Description;
            viewState.DisplayOrder = item.DisplayOrder;
            viewState.PersonalFiles = item.PersonalFiles.Select(a => new AppExePersonalFileViewState()
            {
                PersonalFileId = a.PersonalFileId
            });
            viewState.ImageId = item.ImageId;

            return viewState;
        }
        protected override AppExeViewState CreateDefaultViewState(int lookUpkey)
        {
            var defaultState = ServiceProvider.GetRequiredService<AppExeViewState>();

            defaultState.ExecutableId = lookUpkey;

            defaultState.Caption = "Default name";

            return defaultState;
        }
    }
}
