using StravaIntegration.DTOs;

namespace StravaIntegration.Services;

public interface IStravaActivityService
{
    Task<List<Activity>> GetActivitiesByTypeAsync(string accessToken, ActivityType activityType, int page = 1,
        int perPage = 30);

    Task<StravaActivity[]?> GetActivitiesAsync(string accessToken, int page = 1, int perPage = 30);
    Task<StravaActivity?> GetActivityAsync(string accessToken, long activityId);
    Task<StravaActivityStats?> GetAthleteStatsAsync(string accessToken, long athleteId);
}