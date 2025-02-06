using Microsoft.AspNetCore.Identity;

namespace Core.Domain;

/// <summary>
/// Represents a system user.
/// </summary>
public class User : IdentityUser
{
    [PersonalData] public string FirstName { get; set; } = null!;
    [PersonalData] public string LastName { get; set; } = null!;
    public string? RefreshToken { get; set; }
}