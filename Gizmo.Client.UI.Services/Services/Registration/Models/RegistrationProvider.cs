namespace Gizmo.Client.UI.Services;

public sealed class RegistrationProvider
{
    public Guid PublicId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid ChannelGuid { get; init; }
    public bool CanRedirect { get; init; }
    public bool CanDispatchCode { get; init; }
    public bool CanProvideEmail { get; init; }
    public bool CanProvidePhone { get; init; }
    public bool HasChannel { get; init; }
    public bool Priority { get; init; }
}
