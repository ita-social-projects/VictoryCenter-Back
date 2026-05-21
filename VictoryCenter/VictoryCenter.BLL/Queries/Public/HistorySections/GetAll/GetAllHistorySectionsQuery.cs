using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HistorySection;

namespace VictoryCenter.BLL.Queries.Public.HistorySections.GetAll;

public record GetAllHistorySectionsQuery() : IRequest<Result<List<HistorySectionDto>>>;
