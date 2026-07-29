namespace Gizmo.Client.UI.Services;

public abstract record PasswordRecoveryStartResult
{
    private PasswordRecoveryStartResult() { }

    public sealed record CodeInputRequired(string Token, string? Destination, int CodeLength, int ExpiresInSeconds) : PasswordRecoveryStartResult;
    public sealed record Failed(PasswordRecoveryStartCode Code) : PasswordRecoveryStartResult;
}
