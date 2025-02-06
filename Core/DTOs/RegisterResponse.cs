using Microsoft.AspNetCore.Identity;

namespace Core.DTOs;

public class RegisterResponse : DefaultResponse
{
    public IEnumerable<IdentityError>? Errors { get; set; } = null;
}