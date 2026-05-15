using System.Linq;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services
{
    public sealed class UserRegistrationService : IUserRegistrationService
    {
        private readonly RegistrationsWebApiClient _registrationsClient;
        private readonly TokensWebApiClient _tokensClient;
        private readonly UserAgreementsWebApiClient _agreementsClient;
        private readonly UserGroupsWebApiClient _userGroupsClient;
        private readonly UsersWebApiClient _usersClient;
        private readonly ILogger<UserRegistrationService> _logger;
        private IReadOnlyList<RegistrationProvider>? _cachedProviders;

        public UserRegistrationService(
            RegistrationsWebApiClient registrationsClient,
            TokensWebApiClient tokensClient,
            UserAgreementsWebApiClient agreementsClient,
            UserGroupsWebApiClient userGroupsClient,
            UsersWebApiClient usersClient,
            ILogger<UserRegistrationService> logger)
        {
            _registrationsClient = registrationsClient;
            _tokensClient = tokensClient;
            _agreementsClient = agreementsClient;
            _userGroupsClient = userGroupsClient;
            _usersClient = usersClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<RegistrationProvider>> GetProvidersAsync(CancellationToken ct = default)
        {
            if (_cachedProviders is not null)
                return _cachedProviders;

            var providers = await _registrationsClient.GetProvidersAsync(ct);
            _cachedProviders = providers.Select(RegistrationProviderMapper.Map).ToList();
            return _cachedProviders;
        }

        public async Task<RegistrationRequiredInfo?> GetRequiredUserInfoAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _userGroupsClient.GetAsync(new UserGroupsFilter(), ct);
                var defaultGroup = result.Data?.FirstOrDefault(g => g.IsDefault == true);
                if (defaultGroup?.RequiredUserInfo == null)
                    return null;
                return RegistrationRequiredInfoMapper.Map(defaultGroup.RequiredUserInfo);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetRequiredUserInfoAsync failed, returning null");
                return null;
            }
        }

        public async Task<IReadOnlyList<RegistrationAgreement>> GetAgreementsAsync(CancellationToken ct = default)
        {
            try
            {
                var result = await _agreementsClient.GetAsync(new UserAgreementsFilter { IsEnabled = true }, ct);
                if (result.Data is null)
                    return Array.Empty<RegistrationAgreement>();
                return result.Data.Select(RegistrationAgreementMapper.Map).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GetAgreementsAsync failed, returning empty list");
                return Array.Empty<RegistrationAgreement>();
            }
        }

        public async Task<bool> ExistsAsync(string value, CancellationToken ct = default)
        {
            var result = await _usersClient.UsernameExistAsync(value, ct);
            return result.Id.HasValue;
        }

        public async Task<RegistrationStartResult> StartAsync(RegistrationStartRequest request, CancellationToken ct = default)
        {
            VerificationStartResultModel result;
            string destination;

            if (request.Email != null)
            {
                result = await _registrationsClient.StartAsync(new RegistrationStartModel
                {
                    IntegrationPublicId = request.IntegrationPublicId,
                    DeliveryMethod = RegistrationDeliveryMethodMapper.MapOutbound(request.DeliveryMethod),
                    Email = request.Email
                }, ct);

                destination = MaskEmail(request.Email);
            }
            else
            {
                result = await _registrationsClient.StartAsync(new RegistrationStartModel
                {
                    IntegrationPublicId = request.IntegrationPublicId,
                    DeliveryMethod = RegistrationDeliveryMethodMapper.MapOutbound(request.DeliveryMethod),
                    PhoneNumber = request.Phone!.TrimStart('+')
                }, ct);

                destination = MaskPhone(request.Phone!);
            }

            // R5: API does not return DeliveryMethod — flash-call fallback disabled for MVP
            return new RegistrationStartResult(
                RegistrationStartCodeMapper.Map(result.Result),
                result.Token,
                destination,
                result.CodeLength,
                null,
                result.RedirectUrl,
                result.ExpiresInSeconds);
        }

        private static string MaskEmail(string email)
        {
            var at = email.IndexOf('@');
            if (at <= 1) return email;
            return email[0] + "***" + email[at..];
        }

        private static string MaskPhone(string phone)
        {
            var digits = phone.TrimStart('+');
            if (digits.Length <= 4) return digits;
            return "****" + digits[^4..];
        }

        public async Task<RegistrationConfirmCode> ConfirmTokenAsync(string token, string confirmationCode, CancellationToken ct = default)
            => RegistrationConfirmCodeMapper.Map(await _tokensClient.ConfirmAsync(new TokenConfirmModel { Token = token, ConfirmationCode = confirmationCode }, ct));

        public async Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default)
        {
            var result = await _registrationsClient.ConfirmedAsync(new TokenCheckModel { Token = token }, ct);
            return TokenConfirmedResultMapper.Map(result);
        }

        public async Task<RegistrationCompleteCode> CompleteAsync(RegistrationCompleteRequest request, CancellationToken ct = default)
        {
            var apiProfile = RegistrationProfileMapper.Map(request.Profile);

            // TODO: agreements are collected in ViewState but not sent — new API does not accept them in registration request (R1)
            if (request.Token != null)
            {
                var result = await _registrationsClient.CompleteAsync(
                    new RegistrationCompleteModel { Token = request.Token, Profile = apiProfile, Password = request.Password }, ct);
                return RegistrationCompleteCodeMapper.Map(result);
            }
            else
            {
                var result = await _registrationsClient.DirectAsync(
                    new RegistrationDirectModel { Profile = apiProfile, Password = request.Password }, ct);
                return RegistrationCompleteCodeMapper.Map(result);
            }
        }
    }
}
