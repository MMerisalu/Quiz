using System.ComponentModel.DataAnnotations;
using Base.Domain.Identity;

namespace App.Domain.Identity;

public class AppRole : BaseRole
{
    [Required]
    [MaxLength(128)]
    [StringLength(128, MinimumLength = 1)]
    public string DisplayName { get; set; } = default!;
}