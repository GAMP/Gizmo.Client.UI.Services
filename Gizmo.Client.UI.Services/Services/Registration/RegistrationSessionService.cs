using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

public sealed class RegistrationSessionService : IRegistrationSessionService
{
    public event EventHandler? Changed;

    public string Token { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public int CodeLength { get; private set; }
    public int ExpiresInSeconds { get; private set; }
    public RegistrationFlow Flow { get; private set; }

    public string? ActualContact { get; private set; }
    public string? Country { get; private set; }

    public string Username { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public Sex Sex { get; private set; } = Sex.Unspecified;
    public string? Email { get; private set; }

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

    public void SetProfileBasics(string username, string password,
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
        Password = string.Empty;
        FirstName = null;
        LastName = null;
        BirthDate = null;
        Sex = Sex.Unspecified;
        Email = null;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
