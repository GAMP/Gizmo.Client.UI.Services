using Gizmo.Client.UI.View.States;
using Gizmo.UI.Services;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gizmo.Client.UI.View.Services
{
    [Register()]
    public sealed class PageModuleViewService : ViewStateServiceBase<PageModulesViewState>
    {
        public PageModuleViewService(PageModulesViewState viewState, 
            IUICompositionService uICompositionService,
            IOptionsMonitor<ClientShopOptions> shopOptions,
            IServiceProvider serviceProvider, 
            ILogger<PageModuleViewService> logger)
            :base(viewState,logger,serviceProvider) 
        {
            _shopOptions = shopOptions;
            _uICompositionService = uICompositionService;
        }

        readonly IUICompositionService _uICompositionService;
        readonly IOptionsMonitor<ClientShopOptions> _shopOptions;

        protected override Task OnInitializing(CancellationToken ct)
        {
            var metadata = _uICompositionService.PageModules;

            if (_shopOptions.CurrentValue.Disabled)
            {
                metadata = metadata.Where(md => md.Guid != KnownModules.MODULE_SHOP);
            }

            ViewState.PageModules = metadata
                .Select(module =>
            {
                var vs = ServiceProvider.GetRequiredService<PageModuleViewState>();
                vs.MetaData = module;
                return vs;
            });            

            return base.OnInitializing(ct);
        }
    }
}
