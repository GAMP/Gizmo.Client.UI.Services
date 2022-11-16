using Gizmo.UI.Services;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services.Services
{
    public sealed class ClientDialogService : DialogServiceBase, IClientDialogService
    {
        public ClientDialogService(IServiceProvider serviceProvider, ILogger<ClientDialogService> logger) : base(serviceProvider, logger)
        {
        }
    }
}
