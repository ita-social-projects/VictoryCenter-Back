using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;

namespace VictoryCenter.BLL.Queries.Admin.Localization.History.GetByLanguageId;

public record GetHistoryLocalizationsByLanguageIdQuery(long LanguageId)
    : IRequest<Result<List<HistorySectionLocalizationDto>>>;
