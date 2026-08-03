using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services;

internal static class PasswordRecoveryStartRequestMapper
{
    internal static PasswordRecoveryMethodStartModel Map(PasswordRecoveryStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return request.IdentifierKind switch
        {
            PasswordRecoveryIdentifierKind.Username => new PasswordRecoveryByUsernameMethodStartModel
            {
                MethodId = request.MethodId,
                Value    = request.Value
            },
            PasswordRecoveryIdentifierKind.Email => new PasswordRecoveryByEmailMethodStartModel
            {
                MethodId = request.MethodId,
                Value    = request.Value
            },
            PasswordRecoveryIdentifierKind.MobilePhone => new PasswordRecoveryByMobilePhoneMethodStartModel
            {
                MethodId = request.MethodId,
                Value    = request.Value
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request.IdentifierKind), request.IdentifierKind, "Unsupported password recovery identifier kind.")
        };
    }
}
