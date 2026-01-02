using System.Text;
using System.Text.Json;
using Gizmo.Web.Api.Models;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// User access token provider.
    /// </summary>
    public sealed class UserAccessTokenHandler
    {
        private sealed record AuthTokenState(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAtUtc);

        private readonly Gizmo.Web.Api.User.Clients.AuthWebApiClient _authWebApiClient;
        private static readonly TimeSpan RefreshSkew = TimeSpan.FromMinutes(2);
        private readonly SemaphoreSlim _refreshLock = new(1, 1);
        private volatile AuthTokenState? _state;

        public UserAccessTokenHandler(Gizmo.Web.Api.User.Clients.AuthWebApiClient authWebApiClient)
            => _authWebApiClient = authWebApiClient;

        /// <summary>
        /// Gets current access token.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Access token, null if none present.</returns>
        /// <remarks>
        /// This function returns access token that can be directly used with http request.
        /// </remarks>
        public async ValueTask<string?> GetAsync(CancellationToken cancellationToken = default)
        {
            var state = _state;
            if (state is null)
                return null;

            if (!NeedsRefresh(state))
                return state.AccessToken;

            await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                state = _state;
                if (state is null)
                    return null;

                if (!NeedsRefresh(state))
                    return state.AccessToken;

                var refreshed = await _authWebApiClient.AccessTokenRefreshAsync(
                    new UserAccessTokenRefreshRequestModel
                    {
                        Token = state.AccessToken,
                        RefreshToken = state.RefreshToken
                    },
                    cancellationToken
                ).ConfigureAwait(false);

                var expiresAtUtc = GetJwtExpiresAtUtc(refreshed.Token);

                _state = new AuthTokenState(
                    AccessToken: refreshed.Token,
                    RefreshToken: refreshed.RefreshToken ?? state.RefreshToken,
                    ExpiresAtUtc: expiresAtUtc
                );

                return _state.AccessToken;
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        /// <summary>
        /// Sets current access token.
        /// </summary>
        /// <param name="authToken">Auth token containing access token and refresh token.</param>
        /// <returns></returns>
        /// <remarks>
        /// Set to null to reset current access token.
        /// </remarks>
        public ValueTask SetCurrentAsync(AuthTokenResultModel? authToken)
        {
            // by setting auth token to null we basically reset authentication 
            if(authToken == null)
            {
                _state = null;
                return ValueTask.CompletedTask;
            }

            var expiresAtUtc = GetJwtExpiresAtUtc(authToken.Token);

            _state = new AuthTokenState(
                AccessToken: authToken.Token,
                RefreshToken: authToken.RefreshToken,
                ExpiresAtUtc: expiresAtUtc
            );

            return ValueTask.CompletedTask;
        }

        private static bool NeedsRefresh(AuthTokenState state)
            => DateTimeOffset.UtcNow >= (state.ExpiresAtUtc - RefreshSkew);

        private static DateTimeOffset GetJwtExpiresAtUtc(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2) throw new FormatException("Invalid JWT.");

            static byte[] Base64UrlDecode(string s)
            {
                s = s.Replace('-', '+').Replace('_', '/');
                s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
                return Convert.FromBase64String(s);
            }

            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            using var doc = JsonDocument.Parse(payloadJson);

            if (!doc.RootElement.TryGetProperty("exp", out var expEl) || expEl.ValueKind != JsonValueKind.Number)
                throw new InvalidOperationException("JWT has no exp claim.");

            return DateTimeOffset.FromUnixTimeSeconds(expEl.GetInt64());
        }
    }

}
