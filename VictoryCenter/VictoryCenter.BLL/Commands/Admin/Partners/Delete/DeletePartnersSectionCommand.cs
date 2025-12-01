using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Delete;

public record DeletePartnersSectionCommand(long Id) : IRequest<Result<long>>;
