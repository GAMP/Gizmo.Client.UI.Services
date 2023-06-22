using System.Diagnostics;
using Gizmo.Client.UI.View.Services;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    [Register()]
    public sealed class AppExecutionService : ViewServiceBase
    {   
        public AppExecutionService(IGizmoClient client,
            IClientNotificationService clientNotificationService,
            AppExeViewStateLookupService appExeViewStateLookupService,
            ILogger<AppExecutionService> logger,
            IServiceProvider serviceProvider,
            ILocalizationService localizationService,
            IClientDialogService dialogService) : base(logger, serviceProvider)
        {
            _client = client;
            _clientNotificationService = clientNotificationService;
            _appExeViewStateLookupService = appExeViewStateLookupService;
            _localizationService = localizationService;
            _dialogService = dialogService;
        }

        private readonly IGizmoClient _client;
        private readonly AppExeViewStateLookupService _appExeViewStateLookupService;
        private readonly IClientNotificationService _clientNotificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IClientDialogService _dialogService;

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

                    //pass age rating
                    if (await _client.AppExePassAgeRatingAsync(appExeId, cancellationToken) == false)
                    {
                        _ = await _clientNotificationService.ShowAlertNotification(Gizmo.UI.AlertTypes.Warning,
                           _localizationService.GetString("GIZ_APP_EXE_AGE_RATING_WARNING_TITLE"), _localizationService.GetString("GIZ_APP_EXE_AGE_RATING_WARNING_MESSAGE"),
                           cancellationToken: cancellationToken);
                        return;
                    }

                    //pass execution limit
                    if (await _client.AppExeExecutionLimitPassAsync(appExeId, cancellationToken) == false)
                    {
                        var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_APP_EXE_MAX_LIMIT_WARNING_TITLE"), _localizationService.GetString("GIZ_APP_EXE_MAX_LIMIT_WARNING_MESSAGE"), AlertDialogButtons.YesNo);
                        if (s.Result == AddComponentResultCode.Opened)
                        {
                            var result = await s.WaitForResultAsync();

                            if (s.Result != AddComponentResultCode.Ok || result!.Button != AlertDialogResultButton.Yes)
                                return;

                            //kill all context if required
                            //TODO: AAA DIALOG await Client.ExecutionContextKillAsync();
                        }
                    }

                    if (reprocess)
                    {
                        var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_APP_EXE_REPAIR_VERIFICATION_TITLE"), _localizationService.GetString("GIZ_APP_EXE_REPAIR_VERIFICATION_MESSAGE"), AlertDialogButtons.YesNo);
                        if (s.Result == AddComponentResultCode.Opened)
                        {
                            var result = await s.WaitForResultAsync();

                            if (s.Result != AddComponentResultCode.Ok || result!.Button != AlertDialogResultButton.Yes)
                                return;
                        }
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
                    var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_APP_EXE_ABORT_LAUNCH_VERIFICATION_TITLE"), _localizationService.GetString("GIZ_APP_EXE_ABORT_LAUNCH_VERIFICATION_MESSAGE"), AlertDialogButtons.YesNo);
                    if (s.Result == AddComponentResultCode.Opened)
                    {
                        var result = await s.WaitForResultAsync();

                        if (s.Result == AddComponentResultCode.Ok && result!.Button == AlertDialogResultButton.Yes)
                            await executionContext.AbortAsync();
                    }
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
                    var s = await _dialogService.ShowAlertDialogAsync(_localizationService.GetString("GIZ_APP_EXE_TERMINATE_VERIFICATION_TITLE"), _localizationService.GetString("GIZ_APP_EXE_TERMINATE_VERIFICATION_MESSAGE"), AlertDialogButtons.YesNo);
                    if (s.Result == AddComponentResultCode.Opened)
                    {
                        var result = await s.WaitForResultAsync();

                        if (s.Result == AddComponentResultCode.Ok && result!.Button == AlertDialogResultButton.Yes)
                            await executionContext.TerminateAsync(cancellationToken);
                    }
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
                var personalFilePath = await _client.PersonalFilePathGetAsync(appExeId, personalFileId, cancellationToken);
                if (!string.IsNullOrWhiteSpace(personalFilePath))
                {
                    //try to create personal file directory if one does not exist
                    //in case of failure the error can be logged and there is nothing else that needs to be done
                    if (!Directory.Exists(personalFilePath))
                        Directory.CreateDirectory(personalFilePath);

                    var process = new ProcessStartInfo(personalFilePath)
                    {
                        UseShellExecute = true,
                    };

                    Process.Start(process);
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
                //update view state to the new autolaunch value
                await _appExeViewStateLookupService.SetAutoLaunchAsync(appExeId, autoLaunch);

                var executionContextResult = await _client.AppExeExecutionContextGetAsync(appExeId, cancellationToken);
                var executionContext = executionContextResult.ExecutionContext;
                if (executionContextResult.IsSuccess && executionContext != null)
                {
                    executionContext.AutoLaunch = autoLaunch;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to set autolaunch value for {appExeId}", appExeId);
            }
        }
    }
}
