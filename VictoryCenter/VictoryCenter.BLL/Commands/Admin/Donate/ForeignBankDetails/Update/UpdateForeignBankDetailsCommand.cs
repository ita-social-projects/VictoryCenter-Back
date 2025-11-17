using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Update;

public record UpdateForeignBankDetailsCommand(UpdateForeignBankDetailsDto UpdateForeignBankDetailsDto, long Id)
    : IRequest<Result<ForeignBankDetailsDto>>;
