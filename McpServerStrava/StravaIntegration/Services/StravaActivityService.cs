using StravaIntegration.DTOs;
using StravaIntegration.Http;

namespace StravaIntegration.Services;

public class StravaActivityService : IStravaActivityService
{
    private readonly IStravaClient _stravaClient;

    public StravaActivityService(IStravaClient stravaClient)
    {
        _stravaClient = stravaClient;
    }

    public async Task<List<Activity>> GetActivitiesByTypeAsync(string accessToken, ActivityType activityType,
        int page = 1, int perPage = 30)
    {
        var activities = await GetActivitiesAsync(accessToken, page, perPage);
        return activities?.Where(a => a.Type == activityType.ToString()).Select(a =>
            new Activity(a.Name, a.Distance, a.MovingTime, a.ElapsedTime, a.TotalElevationGain, a.Type,
                a.SportType, a.AverageSpeed, a.MaxSpeed, a.AverageCadence, a.HasHeartrate, a.HeartrateOptOut,
                a.ElevHigh, a.ElevLow, a.StartDateLocal)).OrderByDescending(x => x.StartDateLocal).ToList() ?? [];
    }

    public Task<StravaActivity[]?> GetActivitiesAsync(string accessToken, int page = 1, int perPage = 30)
    {
        var endpoint = $"/api/v3/athlete/activities?page={page}&per_page={perPage}";
        return _stravaClient.GetAsync<StravaActivity[]>(endpoint, accessToken);
    }

    public Task<StravaActivity?> GetActivityAsync(string accessToken, long activityId)
    {
        var endpoint = $"/api/v3/activities/{activityId}";
        return _stravaClient.GetAsync<StravaActivity>(endpoint, accessToken);
    }

    public Task<StravaActivityStats?> GetAthleteStatsAsync(string accessToken, long athleteId)
    {
        var endpoint = $"/api/v3/athletes/{athleteId}/stats";
        return _stravaClient.GetAsync<StravaActivityStats>(endpoint, accessToken);
    }
}