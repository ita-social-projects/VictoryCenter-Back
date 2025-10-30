using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Delete;
public record DeleteForeignBankDetailsCommand(long Id) : IRequest<Result<long>>;
