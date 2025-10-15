using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Donate.SupportOptions.Delete;
public record DeleteSupportOptionsCommand(long Id) : IRequest<Result<long>>;
