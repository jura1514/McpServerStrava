using StravaIntegration.Services;
using System.ComponentModel.DataAnnotations;
using StravaIntegration.DTOs;

namespace StravaIntegration.Endpoints;

public static class StravaActivityEndpoints
{
    public static void MapStravaActivityEndpoints(this WebApplication app)
    {
        var activityGroup = app.MapGroup("/api/strava")
            .WithTags("Strava Activities")
            .WithOpenApi();

        activityGroup.MapGet("/activities",
                async (IStravaActivityService activityService,
                    [Required] string accessToken,
                    int page = 1,
                    int perPage = 30) =>
                {
                    var activities = await activityService.GetActivitiesAsync(accessToken, page, perPage);

                    if (activities == null)
                    {
                        return Results.BadRequest("Failed to retrieve activities. Please check your access token.");
                    }

                    return Results.Ok(activities);
                })
            .WithName("GetStravaActivities")
            .WithSummary("Gets athlete's activities")
            .WithDescription("Retrieves a list of activities for the authenticated athlete filtered.");

        activityGroup.MapGet("/activities/{activityType}",
                async (ActivityType activityType,
                    [Required] string accessToken,
                    IStravaActivityService activityService,
                    int page = 1,
                    int perPage = 30) =>
                {
                    var activities =
                        await activityService.GetActivitiesByTypeAsync(accessToken, activityType, page, perPage);
                    return Results.Ok(activities);
                }
            )
            .WithName("GetStravaActivitiesByType")
            .WithSummary("Gets athlete's activities by type")
            .WithDescription("Retrieves a list of activities for the authenticated athlete filtered by type.");

        activityGroup.MapGet("/activity/{activityId:long}",
                async (long activityId, [Required] string accessToken, IStravaActivityService activityService) =>
                {
                    var activity = await activityService.GetActivityAsync(accessToken, activityId);

                    if (activity == null)
                    {
                        return Results.NotFound(
                            $"Activity with ID {activityId} not found or you don't have permission to access it.");
                    }

                    return Results.Ok(activity);
                })
            .WithName("GetStravaActivity")
            .WithSummary("Gets a specific activity")
            .WithDescription("Retrieves detailed information about a specific activity.");

        activityGroup.MapGet("/athletes/{athleteId:long}/stats",
                async ([Required] long athleteId, [Required] string accessToken,
                    IStravaActivityService activityService) =>
                {
                    var stats = await activityService.GetAthleteStatsAsync(accessToken, athleteId);

                    if (stats == null)
                    {
                        return Results.BadRequest(
                            "Failed to retrieve athlete stats. Please check your access token and permissions.");
                    }

                    return Results.Ok(stats);
                })
            .WithName("GetAthleteStats")
            .WithSummary("Gets athlete statistics")
            .WithDescription("Retrieves activity statistics for the specified athlete.");
    }
}