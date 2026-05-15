using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    internal static class TokenConfirmedResultMapper
    {
        internal static TokenConfirmedResult Map(TokenConfirmedResultModel source) =>
            new()
            {
                IsConfirmed      = source.IsConfirmed,
                Phone            = source.Phone,
                UserAlreadyExists = source.UserAlreadyExists,
            };
    }
}
