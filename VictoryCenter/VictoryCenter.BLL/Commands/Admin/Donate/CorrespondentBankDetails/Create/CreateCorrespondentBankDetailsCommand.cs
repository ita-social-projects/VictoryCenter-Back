using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.CorrespondentBankDetails;

namespace VictoryCenter.BLL.Commands.Admin.Donate.CorrespondentBankDetails.Create;
public record CreateCorrespondentBankDetailsCommand(CreateCorrespondentBankDetailsDto CreateCorrespondentBankDetailsDto)
    : IRequest<Result<CorrespondentBankDetailsDto>>;
