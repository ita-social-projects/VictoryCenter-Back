using FluentResults;
using MediatR;
using VictoryCenter.BLL.DTOs.Admin.HypotherapyProgramCategories;

namespace VictoryCenter.BLL.Queries.Admin.HypotherapyProgramCategories;

public record GetHypotherapyProgramCategoriesQuery : IRequest<Result<List<HypotherapyProgramCategoryDto>>>;
