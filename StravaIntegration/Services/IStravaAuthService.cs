using StravaIntegration.DTOs;

namespace StravaIntegration.Services;

public interface IStravaAuthService
{
    string GetAuthorizationUrl();
    Task<StravaTokenResponse?> GetAccessTokenAsync(string code);
    Task<StravaTokenResponse?> RefreshAccessTokenAsync(string refreshToken);
}