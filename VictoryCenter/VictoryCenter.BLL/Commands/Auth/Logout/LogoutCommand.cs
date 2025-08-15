using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Auth.Logout;

public record LogoutCommand() : IRequest<Result<Unit>>;
