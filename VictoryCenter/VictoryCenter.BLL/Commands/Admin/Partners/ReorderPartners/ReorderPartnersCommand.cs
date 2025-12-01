using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Commands.Admin.Partners.ReorderPartners;

public record ReorderPartnersCommand(ReorderPartnersDto ReorderDto)
    : IRequest<Result<Unit>>;
