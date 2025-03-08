using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Core.Domain;

/// <summary>
/// Represents a system user.
/// </summary>
public class User : IdentityUser
{
    [PersonalData]
    [MaxLength(256)]
    public string FirstName { get; set; } = null!;
    
    [PersonalData] 
    [MaxLength(256)]
    public string LastName { get; set; } = null!;
    
    [MaxLength(50)]
    public string? RefreshToken { get; set; }
}