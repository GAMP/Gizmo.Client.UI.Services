using Gizmo.UI;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.States
{
    [Register()]
    public class UserMenuModuleViewService : ViewStateServiceBase<UserMenuModulesViewState>
    {
        public UserMenuModuleViewService(UserMenuModulesViewState viewState, 
            ILogger<UserMenuModuleViewService> logger, 
            IServiceProvider serviceProvider, IUICompositionService uICompositionService) 
            : base(viewState, logger, serviceProvider)
        {
            _uICompositionService = uICompositionService;
        }
        
        private readonly IUICompositionService _uICompositionService;
        
        protected override Task OnInitializing(CancellationToken ct)
        {
            var metadata = _uICompositionService.UserMenuModules.ToList();

            ViewState.UserMenuModules = metadata
                .Select(module =>
                {
                    var vs = ServiceProvider.GetRequiredService<UserMenuModuleViewState>();
                    vs.MetaData = module;
                    return vs;
                }).ToList();            

            return base.OnInitializing(ct);
        }
    }
}
