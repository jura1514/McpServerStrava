namespace StravaIntegration.DTOs;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    long ExpiresAt,
    int ExpiresIn,
    AthleteResponse? Athlete = null
);

public record AthleteResponse(
    long Id,
    string Username,
    string Firstname,
    string Lastname,
    string Profile
);
