using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Update;

public record UpdateCorrespondentBankDetailsCommand(UpdateCorrespondentBankDetailsDto UpdateCorrespondentBankDetailsDto, long Id)
    : IRequest<Result<CorrespondentBankDetailsDto>>;
