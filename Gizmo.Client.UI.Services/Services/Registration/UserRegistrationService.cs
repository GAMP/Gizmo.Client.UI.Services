using System.Linq;
using Gizmo.Client;
using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.Logging;
using UserTokensWebApiClient = Gizmo.Web.Api.User.Clients.TokensWebApiClient;
using UserUsersWebApiClient = Gizmo.Web.Api.User.Clients.UsersWebApiClient;

namespace Gizmo.Client.UI.Services
{
    public sealed class UserRegistrationService : IUserRegistrationService
    {
        private readonly RegistrationsWebApiClient _registrationsClient;
        private readonly UserTokensWebApiClient _tokensClient;
        private readonly Gizmo.Web.Api.User.Clients.UserAgreementsWebApiClient _agreementsClient;
        private readonly IGizmoClient _gizmoClient;
        private readonly UserUsersWebApiClient _usersClient;
        private readonly ILogger<UserRegistrationService> _logger;
        private IReadOnlyList<RegistrationProvider>? _cachedMethods;

        public UserRegistrationService(
            RegistrationsWebApiClient registrationsClient,
            UserTokensWebApiClient tokensClient,
            Gizmo.Web.Api.User.Clients.UserAgreementsWebApiClient agreementsClient,
            IGizmoClient gizmoClient,
            UserUsersWebApiClient usersClient,
            ILogger<UserRegistrationService> logger)
        {
            _registrationsClient = registrationsClient;
            _tokensClient = tokensClient;
            _agreementsClient = agreementsClient;
            _gizmoClient = gizmoClient;
            _usersClient = usersClient;
            _logger = logger;
        }

        public async Task<IReadOnlyList<RegistrationProvider>> GetMethodsAsync(CancellationToken ct = default)
        {
            if (_cachedMethods is not null)
                return _cachedMethods;

            var methods = await _registrationsClient.GetMethodsAsync(ct);
            _cachedMethods = methods.Select(RegistrationProviderMapper.Map).ToList();
            return _cachedMethods;
        }

        public async Task<RegistrationRequiredInfo?> GetRequiredUserInfoAsync(CancellationToken ct = default)
        {
            try
            {
                var requiredInfo = await _gizmoClient.UserGroupDefaultRequiredInfoGetAsync(ct);
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
                var result = await _agreementsClient.GetAsync(new PublicUserAgreementsFilter(), ct);
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

        public async Task<bool> UserNameExistAsync(string userName, CancellationToken ct = default)
        {
            return await _usersClient.UsernameExistAsync(userName, ct);
        }

        public async Task<bool> EmailExistAsync(string email, CancellationToken ct = default)
        {
            return await _usersClient.EmailExistAsync(email, ct);
        }

        public async Task<bool> MobilePhoneExistAsync(string mobilePhone, CancellationToken ct = default)
        {
            return await _usersClient.MobilePhoneExistAsync(mobilePhone, ct);
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
