using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyLandingPageParticipantsSection;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyLandingPageParticipantsSection.GetByEntityId;

public record GetHippotherapyLandingPageParticipantsSectionLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyLandingPageParticipantsSectionLocalizationDto>>>;
