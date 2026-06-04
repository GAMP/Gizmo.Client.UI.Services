namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoverySessionService : IPasswordRecoverySessionService
{
    public event EventHandler? Changed;

    public PasswordRecoveryProvider? ActiveProvider { get; private set; }

    public string MatchValue { get; private set; } = string.Empty;

    public string Token { get; private set; } = string.Empty;

    public string Destination { get; private set; } = string.Empty;

    public int CodeLength { get; private set; }

    public int ExpiresInSeconds { get; private set; }

    public bool IsCodeConfirmed { get; private set; }

    public void SetActiveProvider(PasswordRecoveryProvider provider)
    {
        ActiveProvider = provider;
    }

    public void SetMatchValue(string value)
    {
        MatchValue = value;
        IsCodeConfirmed = false;
    }

    public void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds)
    {
        Token = token;
        Destination = destination;
        CodeLength = codeLength;
        ExpiresInSeconds = expiresInSeconds;
        IsCodeConfirmed = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetCodeConfirmed(bool value)
    {
        IsCodeConfirmed = value;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        ActiveProvider = null;
        MatchValue = string.Empty;
        Token = string.Empty;
        Destination = string.Empty;
        CodeLength = 0;
        ExpiresInSeconds = 0;
        IsCodeConfirmed = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
