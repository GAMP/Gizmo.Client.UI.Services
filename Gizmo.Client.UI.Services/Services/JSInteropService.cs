using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Java script interop service.
    /// </summary>
    public sealed class JSInteropService : IDisposable
    {
        #region CONSTRUCTOR
        public JSInteropService(IJSRuntime jSRuntime,
            IServiceProvider serviceProvider,
            ILogger<JSInteropService> logger)
        {
            _jsRuntime = jSRuntime;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _objectReference = DotNetObjectReference.Create(this);
        }
        #endregion

        #region FIELDS
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<JSInteropService> _objectReference;
        private readonly ILogger<JSInteropService> _logger;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Java scrip runtime.
        /// </summary>
        private IJSRuntime JSRuntime
        {
            get { return _jsRuntime; }
        }

        /// <summary>
        /// Gets service object reference.
        /// </summary>
        private DotNetObjectReference<JSInteropService> ObjectReference
        {
            get { return _objectReference; }
        }

        #endregion

        #region FUNCTIONS

        [JSInvokable()]
        public Task SetPasswordAsync(string password)
        {
            var state = _serviceProvider.GetRequiredService<View.States.UserLoginViewState>();
            state.Password = password;
            return Task.CompletedTask;
        }

        [JSInvokable()]
        public Task SetUsernameAsync(string username)
        {
            var state = _serviceProvider.GetRequiredService<View.States.UserLoginViewState>();
            state.LoginName = username;
            return Task.CompletedTask;
        }

        [JSInvokable]
        public Task IsFullScreenAsync(bool isFullScreen, string error)
        {
            //TODO: Handle full-screen events
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"FULLSCREEN ERROR: {error}");
            }
            else
            {
                Console.WriteLine($"FULLSCREEN: {isFullScreen}");
            }

            return Task.CompletedTask;
        }
        public async Task InitializeAsync(CancellationToken cToken)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("ClientFunctions.SetDotnetObjectReference", ObjectReference);
                await JSRuntime.InvokeVoidAsync("ClientFunctions.SubscribeOnFullScreenChange", nameof(IsFullScreenAsync));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Could not initalize client javascrip interop.");
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            JSRuntime.InvokeVoidAsync("ClientFunctions.UnsubscribeOnFullScreenChange", nameof(IsFullScreenAsync))
                .AsTask()
                .ContinueWith(
                    task =>
                    {
                        if (task.Exception != null)
                        {
                            throw task.Exception;
                        }
                    });

            _objectReference.Dispose();
        }
        #endregion
    }
}
