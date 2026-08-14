namespace Gizmo.Client.UI.Services;

public sealed class RegistrationProvider
{
    public int MethodId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid ChannelGuid { get; init; }
    public Guid CapabilityGuid { get; init; }
    public bool HasChannel { get; init; }
    public bool IsPrimary { get; init; }
}
