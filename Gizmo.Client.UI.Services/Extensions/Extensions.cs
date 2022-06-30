using Gizmo.Client.UI;
using System.Collections.Concurrent;
using System.Reflection;

namespace Microsoft.AspNetCore.Components
{
    public static class Extensions
    {
        #region READONLY FIELDS
        private static readonly string STATE_HAS_CHANGED_METHOD_NAME = "StateHasChanged";
        private static readonly Type COMPONENT_TYPE = typeof(ComponentBase);
        private static readonly MethodInfo? STATE_HAS_CHANGED_METHOD = COMPONENT_TYPE.GetMethod(STATE_HAS_CHANGED_METHOD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly object?[] DEFAULT_PARAMETERS = Array.Empty<object>();
        private static readonly ConcurrentDictionary<ComponentBase, EventHandler> _delegates = new();
        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Subscribes to the <see cref="IViewState"/> change event and invokes StateHasChanged function on specified component.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <param name="viewState">View state.</param>
        /// <exception cref="ArgumentNullException">thrown if any of the specified parameters is equal to null.</exception>
        public static void SubscribeChange(this ComponentBase component, IViewState viewState)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (viewState == null)
                throw new ArgumentNullException(nameof(viewState));

            //create or get event handler for the component
            var eventHandler = _delegates.GetOrAdd(component, new EventHandler((object? sender, EventArgs e) => {
                STATE_HAS_CHANGED_METHOD?.Invoke(component, DEFAULT_PARAMETERS);
            }));

            //remove any previous handler
            viewState.OnChange -= eventHandler;

            //add handler
            viewState.OnChange += eventHandler;
        }

        /// <summary>
        /// Unsubscribes specified component from the <see cref="IViewState"/> change event.
        /// </summary>
        /// <param name="component">Component.</param>
        /// <param name="viewState">View state.</param>
        /// <exception cref="ArgumentNullException">thrown if any of the specified parameters is equal to null.</exception>
        public static void UnsubscribeChange(this ComponentBase component, IViewState viewState)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if (viewState == null)
                throw new ArgumentNullException(nameof(viewState));

            if(_delegates.TryRemove(component, out var eventHandler))
            {
                viewState.OnChange -= eventHandler;
            }

        } 

        #endregion
    }
}
