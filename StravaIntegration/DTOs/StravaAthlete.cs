namespace StravaIntegration.DTOs;

public record StravaAthlete(
    long Id,
    string Username,
    int ResourceState,
    string Firstname,
    string Lastname,
    string? Bio,
    string City,
    string State,
    string Country,
    string Sex,
    bool Premium,
    bool Summit,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int BadgeTypeId,
    double Weight,
    string ProfileMedium,
    string Profile,
    object? Friend,
    object? Follower
);