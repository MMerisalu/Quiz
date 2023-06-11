using System.ComponentModel.DataAnnotations;

namespace WebApp.DTO;

/// <summary>
/// Register DTO for the all the user types
/// </summary>
public class RegisterDTO
{
    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// User's last name
    /// </summary>
    [Required()]
    [MaxLength(50)]
    [StringLength(50, MinimumLength = 1)]
    
    public string LastName { get; set; } = default!;

    /// <summary>
    /// User's email
    /// </summary>
    [StringLength(50, MinimumLength = 5, ErrorMessage = "Invalid email address length")]
    public string Email { get; set; } = default!;
    /// <summary>
    /// User's password
    /// </summary>
    
        
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    public string[]? Roles { get; set; }
}