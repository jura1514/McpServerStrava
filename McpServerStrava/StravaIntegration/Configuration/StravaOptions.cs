namespace StravaIntegration.Configuration;

public class StravaOptions
{
    public const string SectionName = "Strava";

    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string RedirectUri { get; init; } = string.Empty;
    public string AuthorizationUrl { get; init; } = "https://www.strava.com/oauth/authorize";
    public string TokenUrl { get; init; } = "https://www.strava.com/oauth/token";
    public string[] Scopes { get; init; } = [];
}