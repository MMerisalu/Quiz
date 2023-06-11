using System.ComponentModel.DataAnnotations;
using Base.Domain.Identity;


namespace App.Domain.Identity;

public class AppUser : BaseUser
{
    [Required()]
    [MaxLength(50)]
    public string FirstName { get; set; } = default!;

    [Required()]
    [MaxLength(50)]
    [StringLength(50, MinimumLength = 1)]
    
    public string LastName { get; set; } = default!;

    public string FirstAndLastName => $"{FirstName} {LastName}";

    
    public string LastAndFirstName => $"{LastName} {FirstName}";
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    
    
}
    
