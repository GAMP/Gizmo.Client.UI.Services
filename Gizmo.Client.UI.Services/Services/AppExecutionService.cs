using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    [Register()]
    public sealed class AppExecutionService : ViewServiceBase
    {
        private readonly IGizmoClient _client;
        public AppExecutionService(IGizmoClient client , ILogger<AppExecutionService> logger,
            IServiceProvider serviceProvider):base(logger,serviceProvider)
        {
            _client = client;
        }

        public async Task ExecutAsync(int appExeId, CancellationToken cancellationToken)
        {
            var executionContext = await _client.AppExecutionContextGetAsync(appExeId, cancellationToken);
            if(executionContext.IsSuccess)
            {
               
            }
        }
    }
}
