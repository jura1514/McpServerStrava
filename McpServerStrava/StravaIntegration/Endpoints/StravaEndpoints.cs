namespace StravaIntegration.Endpoints;

public static class StravaEndpoints
{
    public static void MapStravaEndpoints(this WebApplication app)
    {
        app.MapStravaAuthEndpoints();
        app.MapStravaActivityEndpoints();
    }
}