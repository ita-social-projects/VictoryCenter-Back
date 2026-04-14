using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;

namespace VictoryCenter.BLL.Queries.Admin.HistorySections.GetAll;

public record GetAllHistorySectionsQuery() : IRequest<Result<List<HistorySectionDto>>>;
