using Microsoft.AspNetCore.Identity;

namespace Core.DTOs;

public class RegisterResponse
{
    public IEnumerable<IdentityError>? Errors { get; set; } = null;
}