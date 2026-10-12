namespace Gizmo.Client.UI.Services;

/// <summary>
/// Partial update of the logged-in user's profile.
/// A null property is not changed.
/// </summary>
public sealed class UserProfileUpdate
{
    /// <summary>
    /// First name; null leaves the value unchanged.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Last name; null leaves the value unchanged.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Birth date; null leaves the value unchanged.
    /// </summary>
    public DateTime? BirthDate { get; init; }

    /// <summary>
    /// Sex; null leaves the value unchanged.
    /// </summary>
    public UserProfileSex? Sex { get; init; }

    /// <summary>
    /// E-mail address; null leaves the value unchanged.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Mobile phone number in E164 format; null leaves the value unchanged.
    /// </summary>
    public string? MobilePhone { get; init; }

    /// <summary>
    /// Phone number; null leaves the value unchanged.
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Address; null leaves the value unchanged.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// City; null leaves the value unchanged.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Post code; null leaves the value unchanged.
    /// </summary>
    public string? PostCode { get; init; }

    /// <summary>
    /// Country; null leaves the value unchanged.
    /// </summary>
    public string? Country { get; init; }
}
