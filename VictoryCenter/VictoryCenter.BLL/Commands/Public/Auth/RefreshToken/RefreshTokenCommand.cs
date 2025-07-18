using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Public.Auth;

namespace VictoryCenter.BLL.Commands.Public.Auth.RefreshToken;

public record RefreshTokenCommand() : IRequest<Result<AuthResponseDto>>;
