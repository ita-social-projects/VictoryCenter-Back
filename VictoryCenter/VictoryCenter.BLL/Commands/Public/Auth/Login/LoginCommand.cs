using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Auth;

namespace VictoryCenter.BLL.Commands.Public.Auth.Login;

public record LoginCommand(LoginRequestDto RequestDto) : IRequest<Result<AuthResponseDto>>;
