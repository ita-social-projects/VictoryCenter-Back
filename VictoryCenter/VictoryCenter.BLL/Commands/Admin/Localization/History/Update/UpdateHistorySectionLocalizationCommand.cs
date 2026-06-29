using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Update;

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Update;

public record UpdateHistorySectionLocalizationCommand
    (UpdateHistorySectionLocalizationDto UpdateDto,
    long LanguageId) : IRequest<Result<HistorySectionLocalizationDto>>
{
}
