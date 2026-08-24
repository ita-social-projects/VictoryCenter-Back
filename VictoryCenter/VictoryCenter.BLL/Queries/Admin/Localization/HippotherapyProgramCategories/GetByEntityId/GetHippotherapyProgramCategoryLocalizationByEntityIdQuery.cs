using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.Localization.HippotherapyProgramCategories;

namespace VictoryCenter.BLL.Queries.Admin.Localization.HippotherapyProgramCategories.GetByEntityId;

public record GetHippotherapyProgramCategoryLocalizationByEntityIdQuery(long Id)
    : IRequest<Result<List<HippotherapyProgramCategoryLocalizationDto>>>;
