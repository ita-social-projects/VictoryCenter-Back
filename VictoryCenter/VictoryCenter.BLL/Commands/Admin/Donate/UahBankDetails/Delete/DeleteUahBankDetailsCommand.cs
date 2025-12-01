using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Delete;

public record DeleteUahBankDetailsCommand(long Id) : IRequest<Result<long>>;
