using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Update;

public record UpdatePartnerSectionLocalizationCommand(
    UpdatePartnerSectionLocalizationDto UpdatePartnerSectionLocalizationDto,
    long EntityId,
    long LanguageId)
    : IRequest<Result<PartnerSectionLocalizationDto>>;
