using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Auth;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Login;

public record LoginCommand(LoginRequestDto LoginRequestDto) : IRequest<Result<AuthResponseDto>>;
