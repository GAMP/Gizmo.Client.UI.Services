using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Client UI specific view service base.
    /// </summary>
    /// <typeparam name="TState">View state type.</typeparam>
    public abstract class ClientViewServiceBase<TState> : ViewStateServiceBase<TState> where TState : IViewState
    {
        #region CONSTRUCTOR
        public ClientViewServiceBase(TState viewState, 
            ILogger logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
        } 
        #endregion

        #region FIELDS
        private readonly NavigationManager _navigationManager;
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets navigation manager.
        /// </summary>
        protected NavigationManager NavigationManager
        {
            get { return _navigationManager; }
        }

        #endregion

        #region OVERRIDES
        
        protected override Task OnInitializing(CancellationToken ct)
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            base.OnDisposing(isDisposing);
        } 

        #endregion

        #region PROTECTED VIRTUAL

        protected virtual void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
        }      

        #endregion
    }
}
