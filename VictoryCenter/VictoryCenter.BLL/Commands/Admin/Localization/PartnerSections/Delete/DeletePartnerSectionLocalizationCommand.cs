using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.Commands.Admin.Localization.PartnerSections.Delete;

public record DeletePartnerSectionLocalizationCommand(long EntityId, long LanguageId)
    : IRequest<Result<DeletePartnerSectionLocalizationDto>>;
