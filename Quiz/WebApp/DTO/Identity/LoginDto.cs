using System.ComponentModel.DataAnnotations;

namespace WebApp.DTO.Identity;

/// <summary>
/// DTO for login object
/// </summary>
public class LoginDTO
{
    /// <summary>
    /// Email for the login
    /// </summary>
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Invalid email address length")] 
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Password for the login
    /// </summary>
    public string Password { get; set; } = default!;
}