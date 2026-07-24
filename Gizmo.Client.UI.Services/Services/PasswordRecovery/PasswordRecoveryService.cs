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

    public async Task<IReadOnlyList<PasswordRecoveryProvider>> GetProvidersAsync(CancellationToken ct = default)
    {
        var raw = await _recoveriesClient.GetProvidersAsync(matchValue: null, ct);
        var result = new List<PasswordRecoveryProvider>();
        foreach (var p in raw)
        {
            var channel = PasswordRecoveryProviderMapper.TryGetChannel(p.ChannelGuid);
            if (channel is null)
                continue;
            if (!p.CanDispatchCode || p.CanRedirect)
                continue;
            result.Add(PasswordRecoveryProviderMapper.Map(p));
        }

        return result;
    }

    public async Task<PasswordRecoveryStartResult> StartAsync(PasswordRecoveryStartRequest request, CancellationToken ct = default)
    {
        var model = new UserPasswordRecoveryStartModel
        {
            MatchValue = request.MatchValue,
            IntegrationPublicId = request.IntegrationPublicId,
            DeliveryMethod = VerificationDeliveryMethod.CodeDispatch
        };

        var result = await _recoveriesClient.PasswordRecoveryStartAsync(model, ct);
        return PasswordRecoveryStartResultMapper.Map(result, request.MatchValue, request.Channel);
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
