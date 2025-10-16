using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.UahBankDetails;

namespace VictoryCenter.BLL.Commands.Admin.Donate.UahBankDetails.Update;
public record UpdateUahBankDetailsCommand(UpdateUahBankDetailsDto UpdateUahBankDetailsDto, long Id)
    : IRequest<Result<UahBankDetailsDto>>;
