namespace StravaIntegration.DTOs;

public record StravaTokenResponse(
    string TokenType,
    long ExpiresAt,
    int ExpiresIn,
    string RefreshToken,
    string AccessToken,
    StravaAthlete? Athlete = null
);