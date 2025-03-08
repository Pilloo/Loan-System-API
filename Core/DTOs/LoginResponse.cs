namespace Core.DTOs;

/// <summary>
/// Represents the result of the <see cref="Core.UseCases.Handlers.LoginCommandHandler">Login</see>
/// command execution.
/// </summary>
public class LoginResponse
{
    /// <value>The JWT token generated.</value>
    public string Token { get; set; } = string.Empty;

    /// <value>The refresh token used to generate a new JWT token.</value>
    public string RefreshToken { get; set; } = String.Empty;
}