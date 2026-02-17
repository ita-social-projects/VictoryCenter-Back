using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.WhoWeAreContents;

namespace VictoryCenter.BLL.Queries.Admin.Localization.WhoWeAreContents.GetByEntityId;

public record GetWhoWeAreContentLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<WhoWeAreContentLocalizationDto>>>;
