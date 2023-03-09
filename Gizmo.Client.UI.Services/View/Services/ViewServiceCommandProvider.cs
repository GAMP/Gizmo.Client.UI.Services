using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services;

[Register]
public sealed class ViewServiceCommandProvider : ViewServiceBase
{
    private readonly Dictionary<string, IViewService> _services;
    public ViewServiceCommandProvider(
        ILogger<ViewServiceCommandProvider> logger,
        UserCartService userCartService,
        IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
        _services = new()
        {
            {"cart", userCartService }
        };
    }

    public Task ExecuteAsync(IViewServiceCommand command) =>
        _services.ContainsKey(command.Subject)
            ? _services[command.Subject].ExecuteCommandAsync(command)
            : ExecuteCommandAsync(command);
}
