using System.Diagnostics;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    [Register()]
    public sealed class AppExecutionService : ViewServiceBase
    {
        private readonly IGizmoClient _client;
        public AppExecutionService(IGizmoClient client, AppExeViewStateLookupService appExeViewStateLookupService,
            ILogger<AppExecutionService> logger,
            IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
            _client = client;
            _appExeViewStateLookupService = appExeViewStateLookupService;
        }

        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;

        /// <summary>
        /// Executes app.
        /// </summary>
        /// <param name="appExeId">App exe id.</param>
        /// <param name="reprocess">Indicates that we want to start over (repair mode).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task AppExeExecuteAsync(int appExeId, bool reprocess = false, CancellationToken cancellationToken = default)
        {
            return AppExeExecuteAsync(appExeId, reprocess, true, cancellationToken);
        }

        /// <summary>
        /// Executes app.
        /// </summary>
        /// <param name="appExeId">App exe id.</param>
        /// <param name="reprocess">Indicates that we want to start over (repair mode).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task AppExeExecuteAsync(int appExeId, bool reprocess = false, bool autoLaunch = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var executionContextResult = await _client.AppExeExecutionContextGetAsync(appExeId, cancellationToken);
                var executionContext = executionContextResult.ExecutionContext;
                if (executionContextResult.IsSuccess && executionContext != null)
                {
                    if (executionContext.IsExecuting)
                    {
                        //activate app windows
                        await executionContext.TryActivateAsync(cancellationToken);

                        return;
                    }

                    //set auto launch value
                    executionContext.AutoLaunch = autoLaunch;

                    await executionContext.ExecuteAsync(reprocess, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Execution failed for executable {appExeId}", appExeId);
            }
        }

        /// <summary>
        /// Aborts current execution.
        /// </summary>
        /// <param name="appExeId">App exe id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>       
        public async Task AppExeAbortAsync(int appExeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var executionContextResult = await _client.AppExeExecutionContextGetAsync(appExeId, cancellationToken);
                var executionContext = executionContextResult.ExecutionContext;
                if (executionContextResult.IsSuccess && executionContext != null && !executionContext.IsAborting)
                {
                    //TODO show confirmation dialog before proceeding
                    await executionContext.AbortAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Abort failed for executable {appExeId}", appExeId);
            }
        }

        /// <summary>
        /// Terminates any running processes in execitopm context.
        /// </summary>
        /// <param name="appExeId">App exe id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>
        /// Its safe to call this function at any stage of execution as it will only terminate any tracked processes and wont affect any other aspect of execution.
        /// </remarks>
        public async Task AppExeTerminateAsync(int appExeId, CancellationToken cancellationToken = default)
        {
            try
            {
                var executionContextResult = await _client.AppExeExecutionContextGetAsync(appExeId, cancellationToken);
                var executionContext = executionContextResult.ExecutionContext;
                if (executionContextResult.IsSuccess && executionContext != null)
                {
                    await executionContext.TerminateAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Termination failed for executable {appExeId}", appExeId);
            }
        }

        /// <summary>
        /// Explores personal file.
        /// </summary>
        /// <param name="appExeId">App exe id.</param>
        /// <param name="personalFileId">Personal file id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
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

        /// <summary>
        /// Sets auto launch for specified execution context and view state.
        /// </summary>
        /// <param name="appExeId">App exe id.</param>
        /// <param name="autoLaunch">Autolaunch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task SetAutoLaunchAsync(int appExeId, bool autoLaunch, CancellationToken cancellationToken = default)
        {
            try
            {
                var executionContextResult = await _client.AppExeExecutionContextGetAsync(appExeId, cancellationToken);
                var executionContext = executionContextResult.ExecutionContext;
                if (executionContextResult.IsSuccess && executionContext != null)
                {
                    executionContext.AutoLaunch = autoLaunch;

                    //update view state
                    //TODO this is violation as we modify view state outside of service that is responsible of it
                    var viewState = await _appExeViewStateLookupService.GetStateAsync(appExeId, cToken: cancellationToken);
                    viewState.RaiseChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to set autolaunch value for {appExeId}", appExeId);
            }
        }
    }
}
