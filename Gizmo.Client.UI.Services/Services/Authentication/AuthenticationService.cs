using Gizmo.Web.Api.Clients;
using Gizmo.Web.Api.Models;
using Microsoft.Extensions.Logging;

namespace Gizmo.Client.UI.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly Gizmo.Web.Api.User.Clients.AuthWebApiClient _authWebApiClient;
    private readonly UserAccessTokenHandler _userAccessTokenHandler;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        Gizmo.Web.Api.User.Clients.AuthWebApiClient authWebApiClient,
        UserAccessTokenHandler userAccessTokenHandler,
        ILogger<AuthenticationService> logger)
    {
        _authWebApiClient = authWebApiClient;
        _userAccessTokenHandler = userAccessTokenHandler;
        _logger = logger;
    }

    public async Task<AuthenticationLoginResult> LoginAsync(AuthenticationLoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var token = await _authWebApiClient.AccessTokenGetAsync(
                new UserAccessTokenRequestModel
                {
                    Username = request.Username,
                    Password = request.Password ?? string.Empty
                },
                cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(token.Token))
                throw new InvalidOperationException("Authentication response did not include an access token.");

            await _userAccessTokenHandler.SetCurrentAsync(token).ConfigureAwait(false);

            return new AuthenticationLoginResult
            {
                Success = true,
                Error = AuthenticationLoginError.None
            };
        }
        catch (WebApiClientException ex)
        {
            return AuthenticationLoginResultMapper.Map(ex);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Authentication login failed due to a network error.");
            return CreateFailedResult(AuthenticationLoginError.Network, ex.Message);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex, "Authentication login failed due to a network timeout.");
            return CreateFailedResult(AuthenticationLoginError.Network, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authentication login failed.");
            return CreateFailedResult(AuthenticationLoginError.Unknown, ex.Message);
        }
    }

    private static AuthenticationLoginResult CreateFailedResult(AuthenticationLoginError error, string? message)
    {
        return new AuthenticationLoginResult
        {
            Success = false,
            Error = error,
            Message = message
        };
    }
}
