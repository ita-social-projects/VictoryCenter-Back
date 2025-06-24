using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Auth;

namespace VictoryCenter.BLL.Commands.Auth.Login;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
