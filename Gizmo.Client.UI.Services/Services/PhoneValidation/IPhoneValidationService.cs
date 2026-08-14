namespace Gizmo.Client.UI.Services;

public interface IPhoneValidationService
{
    Task<IReadOnlyList<PhoneCountry>> GetCountriesAsync(CancellationToken ct = default);
    Task<PhoneValidationResult> ValidateAsync(string phoneNumber, string regionCode, CancellationToken ct = default);
}
