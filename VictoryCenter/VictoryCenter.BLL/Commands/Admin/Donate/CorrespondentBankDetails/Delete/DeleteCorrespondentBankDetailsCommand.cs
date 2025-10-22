using FluentResults;
using MediatR;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Delete;
public record DeleteCorrespondentBankDetailsCommand(long Id) : IRequest<Result<long>>;
