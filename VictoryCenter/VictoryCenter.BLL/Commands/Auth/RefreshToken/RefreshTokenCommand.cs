using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.BLL.Commands.Auth.RefreshToken;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<Result<AuthResponse>>;
