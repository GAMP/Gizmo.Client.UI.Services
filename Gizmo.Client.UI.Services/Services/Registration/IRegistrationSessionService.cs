using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

public interface IRegistrationSessionService
{
    event EventHandler? Changed;
    event EventHandler? Cleared;

    string Token { get; }
    string Destination { get; }
    int CodeLength { get; }
    int ExpiresInSeconds { get; }
    RegistrationFlow Flow { get; }

    string? ActualContact { get; }
    string? Country { get; }

    string Username { get; }
    string? Password { get; }
    string? FirstName { get; }
    string? LastName { get; }
    DateTime? BirthDate { get; }
    Sex Sex { get; }
    string? Email { get; }
    string? MobilePhone { get; }
    string? Phone { get; }
    string? PhoneE164 { get; }

    bool ShowAllProviders { get; }
    bool AgreementsAccepted { get; }
    IReadOnlyList<RegistrationAgreementChoice> AgreementChoices { get; }

    RegistrationProvider? SelectedProvider { get; }
    RegistrationRequiredInfo? RequiredUserInfo { get; }
    Guid? FailedProviderChannelGuid { get; }

    void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds, RegistrationFlow flow);
    void SetContactDetails(string? actualContact, string? country = null);
    void SetProfileBasics(string username, string? password,
        string? firstName, string? lastName,
        DateTime? birthDate, Sex sex, string? email);
    void SetMobilePhone(string? mobilePhone);
    void SetPhone(string? phone);
    void SetPhoneE164(string? e164);
    void SetShowAllProviders(bool value);
    void SetAgreementsAccepted(bool value);
    void SetAgreementChoices(IEnumerable<RegistrationAgreementChoice> choices);
    void SetSelectedProvider(RegistrationProvider? provider);
    void SetRequiredUserInfo(RegistrationRequiredInfo? info);
    void SetFailedProviderChannelGuid(Guid? channelGuid);
    void Clear();
}
