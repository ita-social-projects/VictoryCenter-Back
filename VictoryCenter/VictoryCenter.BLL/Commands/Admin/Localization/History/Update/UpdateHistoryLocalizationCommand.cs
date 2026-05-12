using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

public record UpdateHistoryLocalizationCommand
    (UpdateHistorySectionLocalizationDto UpdateHistorySectionLocalizationDto,
    long EntityId, long LanguageId) : IRequest<Result<HistorySectionLocalizationDto>>
{
}
