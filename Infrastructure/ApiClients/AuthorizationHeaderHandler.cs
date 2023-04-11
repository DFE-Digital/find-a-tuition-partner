using System.Net.Http.Headers;

namespace Infrastructure.ApiClients;

public class AuthorizationHeaderHandler : DelegatingHandler
{
    private readonly string _accessToken;

    public AuthorizationHeaderHandler(string accessToken)
    {
        _accessToken = accessToken;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        return await base.SendAsync(request, cancellationToken);
    }
}