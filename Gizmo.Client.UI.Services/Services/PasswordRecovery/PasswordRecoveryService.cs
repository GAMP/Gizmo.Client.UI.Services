using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using UserRecoveriesWebApiClient = Gizmo.Web.Api.User.Clients.RecoveriesWebApiClient;
using UserTokensWebApiClient = Gizmo.Web.Api.User.Clients.TokensWebApiClient;

namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoveryService : IPasswordRecoveryService
{
    private readonly UserRecoveriesWebApiClient _recoveriesClient;
    private readonly UserTokensWebApiClient _tokensUserClient;
    private readonly RegistrationsWebApiClient _registrationsClient;
    private readonly VerificationCompleteWebApiClient _verificationCompleteClient;

    public PasswordRecoveryService(
        UserRecoveriesWebApiClient recoveriesClient,
        UserTokensWebApiClient tokensUserClient,
        RegistrationsWebApiClient registrationsClient,
        VerificationCompleteWebApiClient verificationCompleteClient)
    {
        _recoveriesClient = recoveriesClient;
        _tokensUserClient = tokensUserClient;
        _registrationsClient = registrationsClient;
        _verificationCompleteClient = verificationCompleteClient;
    }

    public async Task<IReadOnlyList<PasswordRecoveryProvider>> GetMethodsAsync(CancellationToken ct = default)
    {
        var raw = await _recoveriesClient.GetMethodsAsync(ct);
        return raw.Select(PasswordRecoveryProviderMapper.Map).ToList();
    }

    public async Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default)
    {
        var model = PasswordRecoveryStartRequestMapper.Map(request);
        var result = await _recoveriesClient.PasswordRecoveryStartAsync(model, ct);
        return PasswordRecoveryStartResultMapper.Map(result, request);
    }

    public async Task<PasswordRecoveryConfirmCode> ConfirmCodeAsync(string token, string confirmationCode, CancellationToken ct = default)
    {
        var result = await _tokensUserClient.ConfirmAsync(new TokenConfirmModel
        {
            Token = token,
            ConfirmationCode = confirmationCode
        }, ct);

        return PasswordRecoveryConfirmCodeMapper.Map(result);
    }

    /// <remarks>
    /// Polls the registrations token endpoint - the api exposes no recovery specific
    /// "confirmed" route. Only reachable for redirect/call recovery methods, which no
    /// current server configuration returns, so the endpoint has not been verified
    /// against a password recovery token.
    /// </remarks>
    public async Task<TokenConfirmedResult> IsTokenConfirmedAsync(string token, CancellationToken ct = default)
    {
        var result = await _registrationsClient.ConfirmedAsync(new TokenCheckModel { Token = token }, ct);
        return TokenConfirmedResultMapper.Map(result);
    }

    public async Task<PasswordRecoveryCompleteCode> CompleteAsync(string token, string newPassword, CancellationToken ct = default)
    {
        var result = await _verificationCompleteClient.RecoveryCompleteAsync(new PasswordRecoveryCompleteModel
        {
            Token = token,
            NewPassword = newPassword
        }, ct);

        return PasswordRecoveryCompleteCodeMapper.Map(result);
    }
}
