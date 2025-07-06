using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.BLL.Commands.Auth.Login;

public record LoginCommand(LoginRequestDto RequestDto) : IRequest<Result<AuthResponseDto>>;
