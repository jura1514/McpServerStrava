using StravaIntegration.Services;
using StravaIntegration.DTOs;

namespace StravaIntegration.Endpoints;

public static class StravaAuthEndpoints
{
    public static void MapStravaAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/auth/strava")
            .WithTags("Strava Authentication")
            .WithOpenApi();

        authGroup.MapGet("/login", (IStravaAuthService authService) =>
            {
                var authUrl = authService.GetAuthorizationUrl();
                return Results.Redirect(authUrl);
            })
            .WithName("StravaLogin")
            .WithSummary("Redirects to Strava OAuth login")
            .WithDescription(
                "Redirects the user to Strava's OAuth authorization page. Use this endpoint in Swagger to authenticate with Strava.");

        authGroup.MapGet("/callback", async (string? code, IStravaAuthService authService) =>
            {
                if (string.IsNullOrEmpty(code))
                {
                    return Results.BadRequest("Authorization code is required");
                }

                var tokenResponse = await authService.GetAccessTokenAsync(code);

                if (tokenResponse == null)
                {
                    return Results.BadRequest("Failed to obtain access token from Strava");
                }

                var response = new TokenResponse(
                    tokenResponse.AccessToken,
                    tokenResponse.RefreshToken,
                    tokenResponse.ExpiresAt,
                    tokenResponse.ExpiresIn,
                    tokenResponse.Athlete is not null
                        ? new AthleteResponse(
                            tokenResponse.Athlete.Id,
                            tokenResponse.Athlete.Username,
                            tokenResponse.Athlete.Firstname,
                            tokenResponse.Athlete.Lastname,
                            tokenResponse.Athlete.Profile
                        )
                        : null
                );

                return Results.Ok(response);
            })
            .WithName("StravaCallback")
            .WithSummary("Handles Strava OAuth callback")
            .WithDescription(
                "Processes the authorization code from Strava and exchanges it for access and refresh tokens.");

        authGroup.MapPost("/refresh", async (RefreshTokenRequest request, IStravaAuthService authService) =>
            {
                var tokenResponse = await authService.RefreshAccessTokenAsync(request.RefreshToken);

                if (tokenResponse == null)
                {
                    return Results.BadRequest(
                        "Failed to refresh access token. The refresh token may be invalid or expired.");
                }

                var response = new TokenResponse(
                    tokenResponse.AccessToken,
                    tokenResponse.RefreshToken,
                    tokenResponse.ExpiresAt,
                    tokenResponse.ExpiresIn
                );

                return Results.Ok(response);
            })
            .WithName("RefreshStravaToken")
            .WithSummary("Refreshes Strava access token")
            .WithDescription("Uses a refresh token to obtain a new access token from Strava.");
    }
}