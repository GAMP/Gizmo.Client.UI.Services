using Gizmo.Web.Api.Models;
using Gizmo.Web.Api.User.Clients;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services;

public sealed class PhoneValidationService : IPhoneValidationService
{
    private readonly ValidationsWebApiClient _client;
    private readonly ILogger<PhoneValidationService> _logger;
    private IReadOnlyList<PhoneCountry>? _cachedCountries;

    public PhoneValidationService(ValidationsWebApiClient client, ILogger<PhoneValidationService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PhoneCountry>> GetCountriesAsync(CancellationToken ct = default)
    {
        if (_cachedCountries is not null)
            return _cachedCountries;

        try
        {
            var models = await _client.GetPhoneCountriesAsync(ct);
            _cachedCountries = models.Select(m => new PhoneCountry
            {
                RegionCode  = m.RegionCode,
                CountryName = m.CountryName,
                CallingCode = m.CallingCode,
                InputMask   = m.InputMask,
                Placeholder = m.Placeholder,
                MaxLength   = m.MaxLength,
                Flag        = m.Flag,
            }).ToList();
            return _cachedCountries;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load phone countries from server.");
            return Array.Empty<PhoneCountry>();
        }
    }

    public async Task<PhoneValidationResult> ValidateAsync(string phoneNumber, string regionCode, CancellationToken ct = default)
    {
        try
        {
            var result = await _client.ValidatePhoneAsync(new PhoneValidationRequestModel
            {
                PhoneNumber = phoneNumber,
                RegionCode  = regionCode,
            }, ct);

            if (result.IsValid)
                return new PhoneValidationResult { IsValid = true, E164 = result.E164 };

            var errorKey = result.Result == PhoneValidationResultCode.Error
                ? "GIZ_GEN_AN_ERROR_HAS_OCCURRED"
                : "GIZ_REGISTRATION_VE_MOBILE_PHONE_INVALID";

            return new PhoneValidationResult { IsValid = false, ErrorKey = errorKey };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate phone number.");
            return new PhoneValidationResult { IsValid = false, ErrorKey = "GIZ_GEN_AN_ERROR_HAS_OCCURRED" };
        }
    }
}
