using System.Text.Json;
using Gizmo.UI.View.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.View.Services;

[Register]
public sealed class CommandProviderService : ViewServiceBase
{
    private readonly Dictionary<string, Func<IViewService>> _services;
    public CommandProviderService(
        ILogger<CommandProviderService> logger,
        IServiceProvider serviceProvider) : base(logger, serviceProvider)
    {
        _services = new(StringComparer.OrdinalIgnoreCase)
        {
            {"products/cart", () => serviceProvider.GetRequiredService<UserCartViewStateService>() },
            {"products/details", () => serviceProvider.GetRequiredService<ProductDetailsPageViewStateService>() }
        };
    }

    /// <summary>
    /// Execute the command that will define the service for its and will execute it on that service.
    /// </summary>
    /// <typeparam name="TCommand">Command type that implements IViewServiceCommand interface.</typeparam>
    /// <param name="command">Command from URL.</param>
    /// <param name="cToken">CancellationToken.</param>
    /// <returns>Task of the command.</returns>
    /// <exception cref="NotSupportedException">If the command isn't supported.</exeption>
    public override Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default) =>
        _services.ContainsKey(command.Key)
            ? _services[command.Key]().ExecuteCommandAsync(command, cToken)
            : throw new NotSupportedException(JsonSerializer.Serialize(command));
}
