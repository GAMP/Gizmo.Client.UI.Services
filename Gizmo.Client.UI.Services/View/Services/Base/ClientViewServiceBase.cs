using Gizmo.Client.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services
{
    /// <summary>
    /// Client UI specific view service base.
    /// </summary>
    /// <typeparam name="TViewState">View state type.</typeparam>
    public abstract class ClientViewServiceBase<TViewState> : ViewStateServiceBase<TViewState> where TViewState : IViewState
    {
        #region CONSTRUCTOR
        public ClientViewServiceBase(TViewState viewState, 
            ILogger logger,
            IServiceProvider serviceProvider) : base(viewState, logger, serviceProvider)
        {
            _navigationService = serviceProvider.GetRequiredService<NavigationService>();
        } 
        #endregion

        #region FIELDS
        private readonly NavigationService _navigationService;
        #endregion

        #region PROPERTIES
        
        /// <summary>
        /// Gets navigation service.
        /// </summary>
        protected NavigationService NavigationService
        {
            get { return _navigationService; }
        }

        #endregion

        #region OVERRIDES
        
        protected override Task OnInitializing(CancellationToken ct)
        {
            NavigationService.LocationChanged += OnLocationChanged;
            return base.OnInitializing(ct);
        }

        protected override void OnDisposing(bool isDisposing)
        {
            NavigationService.LocationChanged -= OnLocationChanged;
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
