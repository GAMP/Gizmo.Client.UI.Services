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
            {"products/cart", () => serviceProvider.GetRequiredService<UserCartViewService>() },
            {"products/details", () => serviceProvider.GetRequiredService<ProductDetailsPageViewService>() },
            {"apps/details", () => serviceProvider.GetRequiredService<AppDetailsPageViewService>() }
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
    public override Task ExecuteCommandAsync<TCommand>(TCommand command, CancellationToken cToken = default)
    {
        if (_services.ContainsKey(command.Key))
        {
            return _services[command.Key]().ExecuteCommandAsync(command, cToken);
        }
        else
        {
            throw new NotSupportedException(JsonSerializer.Serialize(command));
        }
    }

    public override bool ValidateCommand<TCommand>(TCommand command)
    {
        if (_services.ContainsKey(command.Key))
        {
            if (_services[command.Key]().ValidateCommand(command))
            {
                return true;
            }
            else
            {
                Logger.LogWarning($"Invalid command: {JsonSerializer.Serialize(command)}");
                return false;
            }
        }
        else
        {
            Logger.LogWarning($"Invalid command: {JsonSerializer.Serialize(command)}");
            return false;
        }
    }
}
