namespace Gizmo.Client.UI.Services;

public abstract record PasswordRecoveryStartResult
{
    private PasswordRecoveryStartResult() { }

    public sealed record CodeInputRequired(string Token, string? Destination, int CodeLength, int ExpiresInSeconds, Guid CapabilityGuid) : PasswordRecoveryStartResult;
    public sealed record RedirectRequired(string Token, string RedirectUrl, int ExpiresInSeconds, Guid CapabilityGuid) : PasswordRecoveryStartResult;
    public sealed record CallRequired(string Token, string PhoneNumber, int ExpiresInSeconds, Guid CapabilityGuid) : PasswordRecoveryStartResult;
    public sealed record Failed(PasswordRecoveryStartCode Code) : PasswordRecoveryStartResult;
}
