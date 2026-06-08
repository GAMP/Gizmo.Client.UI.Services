using System.Threading;
using System.Threading.Tasks;

namespace Gizmo.Client.UI.Services;

public interface IServerInfoService
{
    Task<string?> GetRegionCodeAsync(CancellationToken ct = default);
    Task<string?> GetVersionAsync(CancellationToken ct = default);
    Task<string?> GetDefaultCultureAsync(CancellationToken ct = default);
}
