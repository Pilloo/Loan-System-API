namespace Core.DTOs;

/// <summary>
/// Represents the result of the <see cref="Core.UseCases.Handlers.LoginCommandHandler">Login</see>
/// command execution.
/// </summary>
public class LoginResponse : DefaultResponse
{
    /// <value>The JWT token generated.</value>
    public required string Token { get; set; }

    /// <value>The refresh token used to generate a new JWT token.</value>
    public required string RefreshToken { get; set; }
}