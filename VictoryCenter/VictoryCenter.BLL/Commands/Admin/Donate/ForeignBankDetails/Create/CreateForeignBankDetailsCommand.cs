using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Donate.ForeignBankDetails;

namespace VictoryCenter.BLL.Commands.Admin.Donate.ForeignBankDetails.Create;

public record CreateForeignBankDetailsCommand(CreateForeignBankDetailsDto CreateForeignBankDetailsDto)
    : IRequest<Result<ForeignBankDetailsDto>>;
