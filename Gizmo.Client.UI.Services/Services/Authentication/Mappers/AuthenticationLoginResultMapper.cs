using System.Net;
using Gizmo.Web.Api.Clients;

namespace Gizmo.Client.UI.Services;

public static class AuthenticationLoginResultMapper
{
    public static AuthenticationLoginResult Map(WebApiClientException exception)
    {
        return new AuthenticationLoginResult
        {
            Success = false,
            Error = MapError(exception.HttpStatusCode),
            Message = exception.Message
        };
    }

    private static AuthenticationLoginError MapError(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => AuthenticationLoginError.InvalidCredentials,
            HttpStatusCode.Unauthorized => AuthenticationLoginError.InvalidCredentials,
            HttpStatusCode.Forbidden => AuthenticationLoginError.InvalidCredentials,
            HttpStatusCode.InternalServerError => AuthenticationLoginError.ServerError,
            HttpStatusCode.BadGateway => AuthenticationLoginError.ServerError,
            HttpStatusCode.ServiceUnavailable => AuthenticationLoginError.ServerError,
            HttpStatusCode.GatewayTimeout => AuthenticationLoginError.ServerError,
            _ => AuthenticationLoginError.Unknown
        };
    }
}
