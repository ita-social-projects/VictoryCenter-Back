using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.History;
using VictoryCenter.BLL.DTOs.Admin.Localization.History.Create;

namespace VictoryCenter.BLL.Commands.Admin.Localization.History.Create;

public record CreateHistoryLocalizationCommand(List<CreateHistorySectionLocalizationDto> CreateHistorySectionLocalizationDtos)
    : IRequest<Result<List<HistorySectionLocalizationDto>>>;
