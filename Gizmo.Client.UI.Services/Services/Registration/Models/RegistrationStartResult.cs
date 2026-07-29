namespace Gizmo.Client.UI.Services;

public abstract record RegistrationStartResult
{
    private RegistrationStartResult() { }

    public sealed record CodeInputRequired(string Token, string? Destination, int CodeLength, int ExpiresInSeconds) : RegistrationStartResult;
    public sealed record RedirectRequired(string Token, string RedirectUrl, int ExpiresInSeconds) : RegistrationStartResult;
    public sealed record Failed(RegistrationStartCode Code) : RegistrationStartResult;
}
