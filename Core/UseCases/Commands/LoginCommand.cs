using System.ComponentModel.DataAnnotations;
using Core.DTOs;
using MediatR;
using ErrorHandling;

namespace Core.UseCases.Commands;

public class LoginCommand : IRequest<Result<LoginResponse>>
{
    [Required] public string Username { get; set; } = string.Empty;
    [Required] public string Password { get; set; } = string.Empty;
    [Required] public bool RememberMe { get; set; }
}