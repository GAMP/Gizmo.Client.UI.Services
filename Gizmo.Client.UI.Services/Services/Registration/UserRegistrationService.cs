using System.Linq;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.Logging;
using UserTokensWebApiClient = Gizmo.Web.Api.User.Clients.TokensWebApiClient;
using UserUserGroupsWebApiClient = Gizmo.Web.Api.User.Clients.UserGroupsWebApiClient;

namespace Gizmo.Client.UI.Services
{
    public sealed class UserRegistrationService : IUserRegistrationService
    {
        private readonly RegistrationsWebApiClient _registrationsClient;
        private readonly UserTokensWebApiClient _tokensClient;
        private readonly Gizmo.Web.Api.User.Clients.UserAgreementsWebApiClient _agreementsClient;
        private readonly UserUserGroupsWebApiClient _userGroupsClient;
        private readonly UsersWebApiClient _usersClient;
        private readonly ILogger<UserRegistrationService> _logger;
        private IReadOnlyList<RegistrationProvider>? _cachedProviders;

        public UserRegistrationService(
            RegistrationsWebApiClient registrationsClient,
            UserTokensWebApiClient tokensClient,
            Gizmo.Web.Api.User.Clients.UserAgreementsWebApiClient agreementsClient,
            UserUserGroupsWebApiClient userGroupsClient,
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
                var requiredInfo = await _userGroupsClient.GetDefaultRequiredInfoAsync(ct);
                if (requiredInfo == null)
                    return null;
                return RegistrationRequiredInfoMapper.Map(requiredInfo);
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
            var model  = RegistrationStartRequestMapper.Map(request);
            var result = await _registrationsClient.StartAsync(model, ct);
            return RegistrationStartResultMapper.Map(result, request);
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
            var model  = RegistrationCompleteRequestMapper.MapComplete(request);
            var result = await _registrationsClient.CompleteAsync(model, ct);
            return RegistrationCompleteCodeMapper.Map(result);
        }

        public async Task<RegistrationCompleteCode> DirectAsync(RegistrationCompleteRequest request, CancellationToken ct = default)
        {
            var model  = RegistrationCompleteRequestMapper.MapDirect(request);
            var result = await _registrationsClient.DirectAsync(model, ct);
            return RegistrationCompleteCodeMapper.Map(result);
        }
    }
}
