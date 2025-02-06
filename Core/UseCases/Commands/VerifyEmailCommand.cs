using Core.DTOs;
using Core.Shared;
using MediatR;

namespace Core.UseCases.Commands;

public class VerifyEmailCommand : IRequest<Result<DefaultResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}