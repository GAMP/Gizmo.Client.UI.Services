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

        public async Task ExecutAsync(int appExeId, bool reprocess =false , CancellationToken cancellationToken = default)
        {
            var executionContextResult = await _client.AppExecutionContextGetAsync(appExeId, cancellationToken);
            if(executionContextResult.IsSuccess && executionContextResult.ExecutionContext!=null)
            {
                try
                {
                    await executionContextResult.ExecutionContext.ExecuteAsync(reprocess, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Execution failed for executable {appExeId}", appExeId);
                }
            }
        }
    }
}
