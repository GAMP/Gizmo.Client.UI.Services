namespace Gizmo.Client.UI.Services;

public interface IPasswordRecoverySessionService
{
    event EventHandler? Changed;

    PasswordRecoveryProvider? ActiveProvider { get; }

    string MatchValue { get; }

    string Token { get; }

    string Destination { get; }

    int CodeLength { get; }

    int ExpiresInSeconds { get; }

    bool IsCodeConfirmed { get; }

    Guid? FailedProviderChannelGuid { get; }

    bool ShowAllProviders { get; }

    void SetActiveProvider(PasswordRecoveryProvider provider);

    void SetMatchValue(string value);

    void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds);

    void SetCodeConfirmed(bool value);

    void SetFailedProviderChannelGuid(Guid? channelGuid);

    void SetShowAllProviders(bool value);

    void Clear();
}
