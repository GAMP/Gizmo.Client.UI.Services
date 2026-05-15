namespace Gizmo.Client.UI.Services;

public sealed class TokenConfirmedResult
{
    public bool IsConfirmed { get; init; }
    public string? Phone { get; init; }
    public bool UserAlreadyExists { get; init; }
}
