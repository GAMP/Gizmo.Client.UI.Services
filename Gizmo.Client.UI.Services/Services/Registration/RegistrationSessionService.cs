using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

public sealed class RegistrationSessionService : IRegistrationSessionService
{
    public event EventHandler? Changed;
    public event EventHandler? Cleared;

    public string Token { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public int CodeLength { get; private set; }
    public int ExpiresInSeconds { get; private set; }
    public RegistrationFlow Flow { get; private set; }

    public string? ActualContact { get; private set; }
    public string? Country { get; private set; }

    public bool HasConfirmedMobilePhone =>
        Flow is RegistrationFlow.Sms or RegistrationFlow.Redirect
        && !string.IsNullOrEmpty(ActualContact);

    public string Username { get; private set; } = string.Empty;
    public string? Password { get; private set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public Sex Sex { get; private set; } = Sex.Unspecified;
    public string? Email { get; private set; }
    public string? MobilePhone { get; private set; }
    public string? Phone { get; private set; }
    public string? PhoneE164 { get; private set; }

    public bool ShowAllProviders { get; private set; }
    public bool AgreementsAccepted { get; private set; }
    public IReadOnlyList<RegistrationAgreementChoice> AgreementChoices { get; private set; } = Array.Empty<RegistrationAgreementChoice>();

    public RegistrationProvider? SelectedProvider { get; private set; }
    public RegistrationRequiredInfo? RequiredUserInfo { get; private set; }
    public Guid? FailedProviderChannelGuid { get; private set; }

    public void SetStartResult(string token, string destination, int codeLength, int expiresInSeconds, RegistrationFlow flow)
    {
        Token = token;
        Destination = destination;
        CodeLength = codeLength;
        ExpiresInSeconds = expiresInSeconds;
        Flow = flow;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void SetContactDetails(string? actualContact, string? country = null)
    {
        ActualContact = actualContact;
        Country = country;
    }

    public void SetProfileBasics(string username, string? password,
        string? firstName, string? lastName,
        DateTime? birthDate, Sex sex, string? email)
    {
        Username = username;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Sex = sex;
        Email = email;
    }

    public void SetMobilePhone(string? mobilePhone)
    {
        MobilePhone = mobilePhone;
    }

    public void SetPhone(string? phone)
    {
        Phone = phone;
    }

    public void SetPhoneE164(string? e164)
    {
        PhoneE164 = e164;
    }

    public void SetShowAllProviders(bool value)
    {
        ShowAllProviders = value;
    }

    public void SetAgreementsAccepted(bool value)
    {
        AgreementsAccepted = value;
    }

    public void SetAgreementChoices(IEnumerable<RegistrationAgreementChoice> choices)
    {
        AgreementChoices = choices?.ToList() ?? new();
    }

    public void SetSelectedProvider(RegistrationProvider? provider)
    {
        SelectedProvider = provider;
    }

    public void SetRequiredUserInfo(RegistrationRequiredInfo? info)
    {
        RequiredUserInfo = info;
    }

    public void SetFailedProviderChannelGuid(Guid? channelGuid)
    {
        FailedProviderChannelGuid = channelGuid;
    }

    public void Clear()
    {
        Token = string.Empty;
        Destination = string.Empty;
        CodeLength = 0;
        ExpiresInSeconds = 0;
        Flow = RegistrationFlow.None;
        ActualContact = null;
        Country = null;
        Username = string.Empty;
        Password = null;
        FirstName = null;
        LastName = null;
        BirthDate = null;
        Sex = Sex.Unspecified;
        Email = null;
        MobilePhone = null;
        Phone = null;
        PhoneE164 = null;
        ShowAllProviders = false;
        AgreementsAccepted = false;
        AgreementChoices = Array.Empty<RegistrationAgreementChoice>();
        SelectedProvider = null;
        RequiredUserInfo = null;
        FailedProviderChannelGuid = null;
        Changed?.Invoke(this, EventArgs.Empty);
        Cleared?.Invoke(this, EventArgs.Empty);
    }
}
