using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;

namespace VictoryCenter.BLL.Queries.Admin.Localization.History.GetByEntityId;

public record GetHistoryLocalizationByEntityIdQuery(long EntityId)
    : IRequest<Result<IEnumerable<HistorySectionLocalizationDto>>>;
