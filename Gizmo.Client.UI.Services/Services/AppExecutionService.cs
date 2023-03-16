using System.Diagnostics;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    [Register()]
    public sealed class AppExecutionService : ViewServiceBase
    {
        private readonly IGizmoClient _client;
        public AppExecutionService(IGizmoClient client, ILogger<AppExecutionService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _client = client;
        }

        public async Task AppExeExecuteAsync(int appExeId, bool reprocess = false, CancellationToken cancellationToken = default)
        {
            var executionContextResult = await _client.AppExecutionContextGetAsync(appExeId, cancellationToken);
            var executionContext = executionContextResult.ExecutionContext;
            if (executionContextResult.IsSuccess && executionContext != null)
            {
                try
                {
                    if (executionContext.IsExecuting)
                    {
                        //show already executing for example ?
                        return;
                    }

                    await executionContext.ExecuteAsync(reprocess, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Execution failed for executable {appExeId}", appExeId);
                }
            }
        }

        public async Task AppExeTerminateAsync(int appExeId, CancellationToken cancellationToken = default)
        {
            var executionContextResult = await _client.AppExecutionContextGetAsync(appExeId, cancellationToken);
            var executionContext = executionContextResult.ExecutionContext;
            if (executionContextResult.IsSuccess && executionContext != null)
            {
                try
                {
                    await executionContext.TerminateAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Termination failed for executable {appExeId}", appExeId);
                }
            }
        }

        public async Task PersonalFileExploreAsync(int appExeId, int personalFileId, CancellationToken cancellationToken = default)
        {
            try
            {
                var personalFileExists = await _client.PersonalFileExistAsync(appExeId, personalFileId, cancellationToken);
                if (personalFileExists)
                {
                    var personalFilePath = await _client.PersonalFilePathGetAsync(appExeId, personalFileId, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(personalFilePath))
                    {
                        var process = new ProcessStartInfo(personalFilePath)
                        {
                            UseShellExecute = true,
                        };

                        Process.Start(personalFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Could not execute personal file {personalFileId}, app id {executableId}", personalFileId, appExeId);
            }
        }
    }
}
