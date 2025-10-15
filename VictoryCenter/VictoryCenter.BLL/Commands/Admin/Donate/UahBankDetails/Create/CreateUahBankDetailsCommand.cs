using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Create;
public record CreateUahBankDetailsCommand(CreateUahBankDetailsDto CreateUahBankDetailsDto)
    : IRequest<Result<UahBankDetailsDto>>;
