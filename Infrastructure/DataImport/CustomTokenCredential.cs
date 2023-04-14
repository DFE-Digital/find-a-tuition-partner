using Application.Constants;
using Azure.Core;

namespace Infrastructure.DataImport;

public class CustomTokenCredential : TokenCredential
{
    private readonly string _accessToken;

    public CustomTokenCredential(string accessToken)
    {
        _accessToken = accessToken;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken(_accessToken, DateTimeOffset.UtcNow.AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour));
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new ValueTask<AccessToken>(new AccessToken(_accessToken, DateTimeOffset.UtcNow.AddHours(IntegerConstants.AzureBlobStorageServiceTokenExpiryInHour)));
    }
}