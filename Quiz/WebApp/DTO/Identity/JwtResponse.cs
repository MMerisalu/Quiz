namespace WebApp.DTO.Identity;

/// <summary>
/// Jwt response
/// </summary>
public class JwtResponse
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; } = default!;
    
    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = default!;
    
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = default!;
    
    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; set; } = default!;

    /// <summary>
    /// User's first and last name
    /// </summary>
    public string FirstAndLastName => $"{FirstName} {LastName}";

    /// <summary>
    /// User's last and first name
    /// </summary>
    public string LastAndFirstName => $"{LastName} {FirstName}";
    
    /// <summary>
    /// Role names
    /// </summary>
    public string[] RoleNames { get; set; } = default!;
}