using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Auth.Logout;

public record LogoutCommand : IRequest<Result<Unit>>;
