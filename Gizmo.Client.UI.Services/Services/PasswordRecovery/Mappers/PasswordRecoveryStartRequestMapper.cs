using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartRequestMapper
{
    internal static UserPasswordRecoveryMethodStartModel Map(PasswordRecoveryStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new UserPasswordRecoveryMethodStartModel
        {
            MethodId  = request.MethodId,
            Value     = request.Value,
            ValueKind = ToValueKind(request.IdentifierKind)
        };
    }

    internal static VerificationMethodValueKind ToValueKind(PasswordRecoveryIdentifierKind identifierKind) =>
        identifierKind switch
        {
            PasswordRecoveryIdentifierKind.Username    => VerificationMethodValueKind.Username,
            PasswordRecoveryIdentifierKind.Email       => VerificationMethodValueKind.Email,
            PasswordRecoveryIdentifierKind.MobilePhone => VerificationMethodValueKind.MobilePhone,
            _ => throw new ArgumentOutOfRangeException(nameof(identifierKind), identifierKind, "Unsupported password recovery identifier kind.")
        };
}
