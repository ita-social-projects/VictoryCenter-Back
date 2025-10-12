using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Partners;

namespace VictoryCenter.BLL.Commands.Admin.Partners.UpdateSection;

public record UpdatePartnersSectionCommand(UpdatePartnersSectionDto UpdateDto, long Id)
    : IRequest<Result<PartnersSectionDto>>;
