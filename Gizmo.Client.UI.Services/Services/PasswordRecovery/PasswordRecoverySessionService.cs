namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoverySessionService : IPasswordRecoverySessionService
{
    public event EventHandler? Changed;

    public PasswordRecoveryProvider? ActiveProvider { get; private set; }

    public string MatchValue { get; private set; } = string.Empty;

    public PasswordRecoveryIdentifierKind IdentifierKind { get; private set; }

    public string Token { get; private set; } = string.Empty;

    public string Destination { get; private set; } = string.Empty;

    public string RedirectUrl { get; private set; } = string.Empty;

    public string CallPhoneNumber { get; private set; } = string.Empty;

    public int CodeLength { get; private set; }

    public int ExpiresInSeconds { get; private set; }

    public PasswordRecoveryAction Action { get; private set; }

    public bool IsTokenConfirmed { get; private set; }

    public bool IsCodeConfirmed => IsTokenConfirmed;

    public Guid? FailedProviderChannelGuid { get; private set; }

    public bool ShowAllProviders { get; private set; }

    public void SetActiveProvider(PasswordRecoveryProvider provider)
    {
        ActiveProvider = provider;
    }

    public void SetMatchValue(string value, PasswordRecoveryIdentifierKind identifierKind)
    {
        MatchValue = value;
        IdentifierKind = identifierKind;
        IsTokenConfirmed = false;
    }

    public void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds)
    {
        Token = token;
        Destination = destination;
        RedirectUrl = string.Empty;
        CallPhoneNumber = string.Empty;
        CodeLength = codeLength;
        ExpiresInSeconds = expiresInSeconds;
        Action = PasswordRecoveryAction.Code;
        IsTokenConfirmed = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetRedirectStartResult(string token, string redirectUrl, int expiresInSeconds)
    {
        Token = token;
        Destination = string.Empty;
        RedirectUrl = redirectUrl;
        CallPhoneNumber = string.Empty;
        CodeLength = 0;
        ExpiresInSeconds = expiresInSeconds;
        Action = PasswordRecoveryAction.Redirect;
        IsTokenConfirmed = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetCallStartResult(string token, string phoneNumber, int expiresInSeconds)
    {
        Token = token;
        Destination = string.Empty;
        RedirectUrl = string.Empty;
        CallPhoneNumber = phoneNumber;
        CodeLength = 0;
        ExpiresInSeconds = expiresInSeconds;
        Action = PasswordRecoveryAction.Call;
        IsTokenConfirmed = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetTokenConfirmed(bool value)
    {
        IsTokenConfirmed = value;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetCodeConfirmed(bool value) => SetTokenConfirmed(value);

    public void SetFailedProviderChannelGuid(Guid? channelGuid)
    {
        FailedProviderChannelGuid = channelGuid;
    }

    public void SetShowAllProviders(bool value)
    {
        ShowAllProviders = value;
    }

    public void Clear()
    {
        ActiveProvider = null;
        MatchValue = string.Empty;
        IdentifierKind = PasswordRecoveryIdentifierKind.Username;
        Token = string.Empty;
        Destination = string.Empty;
        RedirectUrl = string.Empty;
        CallPhoneNumber = string.Empty;
        CodeLength = 0;
        ExpiresInSeconds = 0;
        Action = PasswordRecoveryAction.None;
        IsTokenConfirmed = false;
        FailedProviderChannelGuid = null;
        ShowAllProviders = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
