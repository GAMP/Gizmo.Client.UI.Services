namespace Gizmo.Client.UI.Services;

public sealed record PasswordRecoveryStartResult(
    PasswordRecoveryStartCode Result,
    string? Token,
    string? Destination,
    int CodeLength,
    int ExpiresInSeconds);
