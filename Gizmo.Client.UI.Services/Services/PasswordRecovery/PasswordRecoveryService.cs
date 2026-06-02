using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using UserRecoveriesWebApiClient = Gizmo.Web.Api.User.Clients.RecoveriesWebApiClient;
using UserTokensWebApiClient = Gizmo.Web.Api.User.Clients.TokensWebApiClient;

namespace Gizmo.Client.UI.Services;

public sealed class PasswordRecoveryService : IPasswordRecoveryService
{
    private readonly UserRecoveriesWebApiClient _recoveriesClient;
    private readonly UserTokensWebApiClient _tokensUserClient;
    private readonly VerificationCompleteWebApiClient _verificationCompleteClient;

    public PasswordRecoveryService(
        UserRecoveriesWebApiClient recoveriesClient,
        UserTokensWebApiClient tokensUserClient,
        VerificationCompleteWebApiClient verificationCompleteClient)
    {
        _recoveriesClient = recoveriesClient;
        _tokensUserClient = tokensUserClient;
        _verificationCompleteClient = verificationCompleteClient;
    }

    public async Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default)
    {
        var providers = await _recoveriesClient.GetProvidersAsync(request.MatchValue, ct);
        var provider = providers.FirstOrDefault(p => p.CanDispatchCode);

        if (provider is null)
        {
            return new PasswordRecoveryStartResult(
                PasswordRecoveryStartCode.NoRouteForDelivery,
                Token: null,
                Destination: null,
                CodeLength: 0,
                ExpiresInSeconds: 0);
        }

        var model = new UserPasswordRecoveryStartModel
        {
            MatchValue = request.MatchValue,
            IntegrationPublicId = provider.PublicId,
            DeliveryMethod = VerificationDeliveryMethod.CodeDispatch
        };

        var result = await _recoveriesClient.PasswordRecoveryStartAsync(model, ct);
        return PasswordRecoveryStartResultMapper.Map(result, request.MatchValue);
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
