namespace Gizmo.Client.UI.Services;

public interface IUserProfileService
{
    /// <summary>
    /// Raised after a successful <see cref="UpdateAsync"/> with the saved values; a null value was not changed.
    /// </summary>
    event EventHandler<UserProfileUpdate>? Updated;

    /// <summary>
    /// Saves the current user's profile; only non-null values are sent.
    /// Returns the outcome code and never throws for known server outcomes.
    /// Raises <see cref="Updated"/> on success.
    /// </summary>
    Task<UserProfileUpdateCode> UpdateAsync(UserProfileUpdate update, CancellationToken ct = default);

    /// <summary>
    /// Returns true when the e-mail address is already used by a user account; throws on request failure.
    /// </summary>
    Task<bool> EmailExistAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Returns true when the mobile phone (digits without the leading '+') is already used by a user account; throws on request failure.
    /// </summary>
    Task<bool> MobilePhoneExistAsync(string mobilePhone, CancellationToken ct = default);
}
