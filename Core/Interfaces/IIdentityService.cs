using Core.Domain;

namespace Core.Interfaces;

public interface IIdentityService
{
    Task<String> GenerateAccessToken(User user);
    Task<String> GenerateRefreshToken();
}