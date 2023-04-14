namespace Application.Common.Interfaces;

public interface IGetAccessToken
{
    Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string tenantId, string resource);
}