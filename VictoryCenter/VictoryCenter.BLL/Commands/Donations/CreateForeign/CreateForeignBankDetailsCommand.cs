using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.BLL.DTOs.Donations.ForeignBank;

namespace VictoryCenter.BLL.Commands.Donations.CreateForeign;

public record CreateForeignBankDetailsCommand(CreateForeignBankDetailsDto CreateForeignBankDetailsDto) : IRequest<Result<BankDetailsDto>>;
