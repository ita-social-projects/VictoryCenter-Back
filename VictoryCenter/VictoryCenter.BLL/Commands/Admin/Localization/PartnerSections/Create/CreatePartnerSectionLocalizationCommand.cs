using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Create;

public record CreatePartnerSectionLocalizationCommand(CreatePartnerSectionLocalizationDto CreatePartnerSectionLocalizationDto)
    : IRequest<Result<PartnerSectionLocalizationDto>>;
