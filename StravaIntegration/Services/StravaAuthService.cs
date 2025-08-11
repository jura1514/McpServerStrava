using Microsoft.Extensions.Options;
using System.Web;
using StravaIntegration.Configuration;
using StravaIntegration.DTOs;
using StravaIntegration.Http;

namespace StravaIntegration.Services;

public class StravaAuthService : IStravaAuthService
{
    private readonly IStravaClient _stravaClient;
    private readonly StravaOptions _options;

    public StravaAuthService(IStravaClient stravaClient, IOptions<StravaOptions> options)
    {
        _stravaClient = stravaClient;
        _options = options.Value;
    }

    public string GetAuthorizationUrl()
    {
        var queryParams = HttpUtility.ParseQueryString(string.Empty);
        queryParams["client_id"] = _options.ClientId;
        queryParams["response_type"] = "code";
        queryParams["redirect_uri"] = _options.RedirectUri;
        queryParams["scope"] = string.Join(",", _options.Scopes);
        queryParams["approval_prompt"] = "force";

        return $"{_options.AuthorizationUrl}?{queryParams}";
    }

    public Task<StravaTokenResponse?> GetAccessTokenAsync(string code)
    {
        var requestData = new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code"
        };

        return _stravaClient.PostFormAsync<StravaTokenResponse>(_options.TokenUrl, requestData);
    }

    public Task<StravaTokenResponse?> RefreshAccessTokenAsync(string refreshToken)
    {
        var requestData = new Dictionary<string, string>
        {
            ["client_id"] = _options.ClientId,
            ["client_secret"] = _options.ClientSecret,
            ["refresh_token"] = refreshToken,
            ["grant_type"] = "refresh_token"
        };

        return _stravaClient.PostFormAsync<StravaTokenResponse>(_options.TokenUrl, requestData);
    }
}
