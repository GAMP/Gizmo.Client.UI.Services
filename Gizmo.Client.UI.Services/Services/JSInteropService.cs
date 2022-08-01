using Microsoft.JSInterop;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// Java script interop service.
    /// </summary>
    public class JSInteropService : IDisposable
    {
        #region CONSTRUCTOR
        public JSInteropService(IJSRuntime jSRuntime, View.States.UserLoginViewState userLoginViewState)
        {
            _jsRuntime = jSRuntime;
            _objectReference = DotNetObjectReference.Create(this);
            State = userLoginViewState;
        }
        #endregion

        #region FIELDS
        private readonly string FUNCTION_CLASS_PREFIX = "UIFunctions";
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<JSInteropService> _objectReference;
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

        View.States.UserLoginViewState State
        {
            get;set;
        }

        [JSInvokable(nameof(SetNumberAsync))]
        public Task SetNumberAsync(int number)
        {
            State.LoginName = number.ToString();
            State.RaiseChanged();
            return Task.CompletedTask;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync($"{FUNCTION_CLASS_PREFIX}.{nameof(SetNumberAsync)}", ObjectReference);
            }
            catch (JSException jsex)
            {
               //js error
            }
            catch (Exception)
            {

            }
        }

        public void Dispose()
        {
            _objectReference.Dispose();
        }
    }
}
