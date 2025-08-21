namespace StravaMcpServer.Configuration;

public class StravaConfiguration
{
    public const string SectionName = "Strava";
    public required string AccessToken { get; init; }
    public required Uri BaseUrl { get; init; } = new("http://localhost:5033");
}