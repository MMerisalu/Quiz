

namespace WebApp.DTO.Identity;

/// <summary>
/// Model for refresh token DTO
/// </summary>
public class RefreshTokenModelDTO
{
    /// <summary>
    /// Old token
    /// </summary>
    public string Token { get; set; } = default!;
    
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = default!;
}