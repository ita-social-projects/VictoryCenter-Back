using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.PartnerSections;

namespace VictoryCenter.BLL.Queries.Admin.Localization.PartnerSections.GetByLanguageId;

public record GetPartnerSectionLocalizationByLanguageIdQuery(long EntityId, long LanguageId)
    : IRequest<Result<PartnerSectionLocalizationDto>>;
