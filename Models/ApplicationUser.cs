using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CorporateCMS.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)]
    public string? DisplayName { get; set; }

    [MaxLength(500)]
    public string? Bio { get; set; }

    [MaxLength(256)]
    public string? AvatarUrl { get; set; }

    public DateTime? LastLoginAt { get; set; }
}
