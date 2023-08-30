namespace Application.Common.Interfaces;

public interface IGenerateUserDelegationSasTokenAsync
{
    Task<string> GenerateUserDelegationSasTokenAsync();
}