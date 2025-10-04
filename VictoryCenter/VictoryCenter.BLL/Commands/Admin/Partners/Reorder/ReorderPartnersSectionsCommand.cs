using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Commands.Admin.Partners.Reorder;

public record ReorderPartnersSectionsCommand(ReorderPartnersSectionsDto ReorderDto)
    : IRequest<Result<Unit>>;
