using System.Net.Http.Headers;

namespace Gizmo.Client.UI.Services
{
    /// <summary>
    /// User http api delegating handler.
    /// </summary>
    /// <remarks>
    /// This handler is used to inject bearer tokens into http client requests.
    /// </remarks>
    public sealed class UserApiClientDelegatingHandler : DelegatingHandler
    {
        public UserApiClientDelegatingHandler(UserAccessTokenHandler accessTokenHandler) 
        {
            _accessTokenHandler = accessTokenHandler;
        }

        private readonly UserAccessTokenHandler _accessTokenHandler;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _accessTokenHandler.GetAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(accessToken) && request.Headers.Authorization is null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
