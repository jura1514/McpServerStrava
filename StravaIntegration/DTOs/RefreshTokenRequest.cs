using System.ComponentModel.DataAnnotations;

namespace StravaIntegration.DTOs;

public record RefreshTokenRequest(
    [Required(ErrorMessage = "Refresh token is required")]
    [MinLength(1, ErrorMessage = "Refresh token cannot be empty")]
    string RefreshToken
);