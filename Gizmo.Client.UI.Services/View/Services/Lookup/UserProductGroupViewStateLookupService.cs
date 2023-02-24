using Gizmo.Client.UI.View.States;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class UserProductGroupViewStateLookupService : ViewStateLookupServiceBase<int,UserProductGroupViewState>
    {
        public UserProductGroupViewStateLookupService(IGizmoClient gizmoClient, 
            ILogger<UserProductGroupViewStateLookupService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _gizmoClient = gizmoClient;
        }

        private readonly IGizmoClient _gizmoClient;

        protected override Task<bool> DataInitializeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        protected override ValueTask<UserProductGroupViewState> CreateViewStateAsync(int lookUpkey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        protected override UserProductGroupViewState CreateDefaultViewStateAsync(int lookUpkey)
        {
            return base.CreateDefaultViewStateAsync(lookUpkey);
        }
    }
}
