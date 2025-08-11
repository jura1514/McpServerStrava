namespace StravaIntegration.DTOs;

public record StravaActivityStats(
    StravaActivityTotal RecentRideTotal,
    StravaActivityTotal RecentRunTotal,
    StravaActivityTotal RecentSwimTotal,
    StravaActivityTotal YtdRideTotal,
    StravaActivityTotal YtdRunTotal,
    StravaActivityTotal YtdSwimTotal,
    StravaActivityTotal AllRideTotal,
    StravaActivityTotal AllRunTotal,
    StravaActivityTotal AllSwimTotal
);

public record StravaActivityTotal(
    int Count,
    double Distance,
    int MovingTime,
    int ElapsedTime,
    double ElevationGain,
    int AchievementCount
);