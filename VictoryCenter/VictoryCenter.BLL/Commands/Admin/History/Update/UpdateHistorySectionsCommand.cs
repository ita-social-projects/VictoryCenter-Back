using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;

namespace VictoryCenter.BLL.Commands.Admin.History.Update;

public record UpdateHistorySectionsCommand(List<UpdateHistorySectionDto> UpdateSections) : IRequest<Result<List<HistorySectionDto>>>;
