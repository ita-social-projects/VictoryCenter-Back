using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.EventNewsCategories;

namespace VictoryCenter.BLL.Queries.Admin.Localization.EventNewsCategories.GetByEntityId;

public record GetEventNewsCategoryLocalizationsByEntityIdQuery(long EntityId)
    : IRequest<Result<List<AdminEventNewsCategoryLocalizationDto>>>;
