using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Auth;

namespace VictoryCenter.BLL.Commands.Admin.Auth.RefreshToken;

public record RefreshTokenCommand : IRequest<Result<AuthResponseDto>>;
