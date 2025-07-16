using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.BLL.Commands.Auth.RefreshToken;

public record RefreshTokenCommand() : IRequest<Result<AuthResponseDto>>;
