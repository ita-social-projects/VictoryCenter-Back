using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Donations.Common;
using VictoryCenter.BLL.DTOs.Donations.UahBank;

namespace VictoryCenter.BLL.Commands.Donations.CreateUah;

public record CreateUahBankDetailsCommand(CreateUahBankDetailsDto CreateUahBankDetailsDto) : IRequest<Result<BankDetailsDto>>;
