using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.BLL.Commands.Auth.RefrehsToken;

public record RefreshTokenCommand(string ExpiredAccessToken, string RefreshToken) : IRequest<Result<AuthResponse>>;
